using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.Middleware;
using AuditoriaRecepcion.Services.Interfaces;
using AuditoriaRecepcion.Services.Implementation;
using AuditoriaRecepcion.Repositories.Interfaces;
using AuditoriaRecepcion.Repositories.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURACIÓN DE SERILOG (LOGGING)
// ============================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "AuditoriaRecepcion")
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// ============================================
// CONFIGURACIÓN DE BASE DE DATOS
// ============================================
builder.Services.AddDbContext<AuditoriaRecepcionContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(60);
        });

    // Habilitar logs detallados solo en desarrollo
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// ============================================
// CONFIGURACIÓN DE AUTENTICACIÓN JWT
// ============================================
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ============================================
// CONFIGURACIÓN DE CORS
// ============================================
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ============================================
// INYECCIÓN DE DEPENDENCIAS - SERVICIOS
// ============================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();
builder.Services.AddScoped<IIncidenciaService, IncidenciaService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// ============================================
// INYECCIÓN DE DEPENDENCIAS - REPOSITORIOS
// ============================================
builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IIncidenciaRepository, IncidenciaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// ============================================
// CONFIGURACIÓN DE CONTROLADORES
// ============================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // PascalCase
    });

// ============================================
// CONFIGURACIÓN DE SWAGGER
// ============================================
if (builder.Configuration.GetValue<bool>("Application:EnableSwagger"))
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "API de Auditoría de Recepción",
            Version = "v1",
            Description = "API para gestión de auditorías de recepción de mercadería",
            Contact = new OpenApiContact
            {
                Name = "Equipo de Desarrollo",
                Email = "desarrollo@auditoria-recepcion.com"
            }
        });

        // Configuración de autenticación JWT en Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Ingrese el token JWT en el formato: Bearer {token}",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Incluir comentarios XML (opcional)
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });
}

// ============================================
// CONFIGURACIÓN DE HTTPCONTEXT ACCESSOR
// ============================================
builder.Services.AddHttpContextAccessor();

// ============================================
// CONFIGURACIÓN DE RATE LIMITING
// ============================================
if (builder.Configuration.GetValue<bool>("RateLimiting:EnableRateLimiting"))
{
    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            return System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                factory: partition => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit"),
                    Window = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("RateLimiting:Window")),
                    QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst,
                    QueueLimit = builder.Configuration.GetValue<int>("RateLimiting:QueueLimit")
                });
        });

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });
}

// ============================================
// CONSTRUCCIÓN DE LA APLICACIÓN
// ============================================
var app = builder.Build();

// ============================================
// APLICAR MIGRACIONES AUTOMÁTICAMENTE (OPCIONAL)
// ============================================
if (builder.Configuration.GetValue<bool>("Application:AutoMigrate", false))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AuditoriaRecepcionContext>();
        try
        {
            dbContext.Database.Migrate();
            Log.Information("Migraciones aplicadas exitosamente");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al aplicar migraciones");
        }
    }
}

// ============================================
// CONFIGURACIÓN DEL PIPELINE DE MIDDLEWARE
// ============================================

// 1. Manejo global de errores (debe ir primero)
app.UseErrorHandling();

// 2. HTTPS Redirection
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// 3. Archivos estáticos (si se necesitan)
app.UseStaticFiles();

// 4. Routing
app.UseRouting();

// 5. CORS (antes de autenticación)
app.UseCors("AllowFrontend");

// 6. Rate Limiting
if (builder.Configuration.GetValue<bool>("RateLimiting:EnableRateLimiting"))
{
    app.UseRateLimiter();
}

// 7. Logging de requests
app.UseRequestLogging();

// 8. Performance monitoring
if (builder.Configuration.GetValue<bool>("Performance:EnablePerformanceMonitoring"))
{
    app.UsePerformanceMonitoring();
}

// 9. Autenticación y Autorización
app.UseAuthentication();
app.UseJwtMiddleware(); // Middleware personalizado JWT
app.UseAuthorization();

// 10. Swagger (solo en desarrollo o si está habilitado)
if (app.Configuration.GetValue<bool>("Application:EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Auditoría de Recepción v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "API de Auditoría de Recepción - Documentación";
    });
}

// 11. Mapeo de controladores
app.MapControllers();

// ============================================
// ENDPOINT DE SALUD (HEALTH CHECK)
// ============================================
app.MapGet("/api/health", () => Results.Ok(new
{
    Status = "Healthy",
    Application = builder.Configuration["Application:Name"],
    Version = builder.Configuration["Application:Version"],
    Environment = builder.Configuration["Application:Environment"],
    Timestamp = DateTime.UtcNow
}))
.WithName("HealthCheck")
.WithTags("Health");

// ============================================
// INFORMACIÓN DE LA API (ROOT ENDPOINT)
// ============================================
app.MapGet("/", () => Results.Ok(new
{
    Application = builder.Configuration["Application:Name"],
    Version = builder.Configuration["Application:Version"],
    Environment = builder.Configuration["Application:Environment"],
    Documentation = "/swagger",
    Health = "/api/health",
    Timestamp = DateTime.UtcNow
}))
.ExcludeFromDescription();

// ============================================
// LOGGING DE INICIO
// ============================================
Log.Information("===========================================");
Log.Information("Iniciando aplicación: {Application}", builder.Configuration["Application:Name"]);
Log.Information("Versión: {Version}", builder.Configuration["Application:Version"]);
Log.Information("Entorno: {Environment}", app.Environment.EnvironmentName);
Log.Information("===========================================");

// ============================================
// INICIAR LA APLICACIÓN
// ============================================
try
{
    app.Run();
    Log.Information("Aplicación finalizada correctamente");
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación terminó inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}