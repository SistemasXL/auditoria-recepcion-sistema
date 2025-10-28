using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.DTOs.Usuario;
using AuditoriaRecepcion.DTOs.Auth;
using AuditoriaRecepcion.DTOs.Common;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AuditoriaRecepcion.Services.Implementation
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AuditoriaRecepcionContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(
            AuditoriaRecepcionContext context,
            IEmailService emailService,
            ILogger<UsuarioService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<PaginatedResult<UsuarioDTO>> GetUsuariosAsync(UsuarioFiltroDTO filtro)
        {
            try
            {
                var query = _context.Usuarios
                    .AsNoTracking()
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(filtro.Busqueda))
                {
                    query = query.Where(u =>
                        u.NombreUsuario.Contains(filtro.Busqueda) ||
                        u.NombreCompleto.Contains(filtro.Busqueda) ||
                        u.Email.Contains(filtro.Busqueda));
                }

                if (!string.IsNullOrEmpty(filtro.Rol))
                    query = query.Where(u => u.Rol == filtro.Rol);

                if (filtro.Activo.HasValue)
                    query = query.Where(u => u.Activo == filtro.Activo.Value);

                // Contar total
                var totalItems = await query.CountAsync();

                // Ordenar y paginar
                var items = await query
                    .OrderBy(u => u.NombreCompleto)
                    .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                    .Take(filtro.PageSize)
                    .Select(u => MapToUsuarioDTO(u))
                    .ToListAsync();

                return new PaginatedResult<UsuarioDTO>(items, totalItems, filtro.PageNumber, filtro.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                throw;
            }
        }

        public async Task<UsuarioDetalleDTO> GetUsuarioByIdAsync(int id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UsuarioID == id);

                if (usuario == null)
                    return null;

                // Obtener estadísticas
                var totalAuditorias = await _context.AuditoriasRecepcion
                    .CountAsync(a => a.UsuarioAuditorID == id);

                var totalIncidenciasDetectadas = await _context.Incidencias
                    .CountAsync(i => i.UsuarioReportaID == id);

                var totalIncidenciasAsignadas = await _context.Incidencias
                    .CountAsync(i => i.UsuarioResponsableID == id);

                var incidenciasResueltasAsignadas = await _context.Incidencias
                    .CountAsync(i => i.UsuarioResponsableID == id && i.EstadoIncidencia == "Resuelta");

                return new UsuarioDetalleDTO
                {
                    UsuarioID = usuario.UsuarioID,
                    Usuario = usuario.NombreUsuario,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email,
                    Rol = usuario.Rol,
                    Activo = usuario.Activo,
                    FechaCreacion = usuario.FechaCreacion,
                    UltimoAcceso = usuario.UltimoAcceso,
                    // CreadoPor no existe como navegación en Usuario
                    TotalAuditorias = totalAuditorias,
                    TotalIncidenciasDetectadas = totalIncidenciasDetectadas,
                    TotalIncidenciasAsignadas = totalIncidenciasAsignadas,
                    IncidenciasResueltasAsignadas = incidenciasResueltasAsignadas
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {Id}", id);
                throw;
            }
        }

        public async Task<UsuarioDTO> CreateUsuarioAsync(CreateUsuarioDTO dto, int adminId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar usuario único
                if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == dto.Usuario))
                    throw new InvalidOperationException($"Ya existe un usuario con el nombre: {dto.Usuario}");

                // Validar email único
                if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                    throw new InvalidOperationException($"Ya existe un usuario con el email: {dto.Email}");

                var usuario = new Usuario
                {
                    NombreUsuario = dto.Usuario,
                    NombreCompleto = dto.NombreCompleto,
                    Email = dto.Email,
                    Rol = dto.Rol,
                    ContrasenaHash = HashPassword(dto.Contrasena),
                    Activo = dto.Activo,
                    FechaCreacion = DateTime.Now
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(adminId, "Crear", "Usuario", usuario.UsuarioID);

                // Enviar email de bienvenida
                await _emailService.SendWelcomeEmailAsync(usuario.Email, usuario.NombreUsuario, dto.Contrasena);

                await transaction.CommitAsync();

                _logger.LogInformation("Usuario creado: {Usuario} por administrador {AdminId}", dto.Usuario, adminId);

                return MapToUsuarioDTO(usuario);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al crear usuario");
                throw;
            }
        }

        public async Task<UsuarioDTO> UpdateUsuarioAsync(int id, UpdateUsuarioDTO dto, int adminId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                    return null;

                // Validar email único si cambió
                if (dto.Email != usuario.Email &&
                    await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.UsuarioID != id))
                    throw new InvalidOperationException($"Ya existe un usuario con el email: {dto.Email}");

                usuario.NombreCompleto = dto.NombreCompleto;
                usuario.Email = dto.Email;
                usuario.Rol = dto.Rol;
                usuario.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(adminId, "Actualizar", "Usuario", id);

                await transaction.CommitAsync();

                _logger.LogInformation("Usuario actualizado: {Id} por administrador {AdminId}", id, adminId);

                return MapToUsuarioDTO(usuario);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al actualizar usuario {Id}", id);
                throw;
            }
        }

        public async Task ToggleUsuarioStatusAsync(int id, int adminId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                    throw new InvalidOperationException("Usuario no encontrado");

                // No permitir desactivar al propio administrador
                if (id == adminId && usuario.Activo)
                    throw new InvalidOperationException("No puedes desactivar tu propio usuario");

                usuario.Activo = !usuario.Activo;
                usuario.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(adminId, "CambiarEstado", "Usuario", id);

                await transaction.CommitAsync();

                _logger.LogInformation("Estado de usuario cambiado: {Id} a {Estado} por administrador {AdminId}",
                    id, usuario.Activo ? "Activo" : "Inactivo", adminId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al cambiar estado del usuario {Id}", id);
                throw;
            }
        }

        public async Task<ResetPasswordResponseDTO> ResetPasswordAsync(int id, int adminId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                    throw new InvalidOperationException("Usuario no encontrado");

                // Generar contraseña temporal
                var newPassword = GenerateTemporaryPassword();

                usuario.ContrasenaHash = HashPassword(newPassword);
                usuario.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(adminId, "ResetearContrasena", "Usuario", id);

                // Enviar email
                await _emailService.SendPasswordResetEmailAsync(usuario.Email, usuario.NombreUsuario, newPassword);

                await transaction.CommitAsync();

                _logger.LogInformation("Contraseña reseteada para usuario: {Id} por administrador {AdminId}", id, adminId);

                return new ResetPasswordResponseDTO
                {
                    Success = true,
                    NuevaContrasena = newPassword,
                    Mensaje = "Contraseña reseteada exitosamente. Se ha enviado un email al usuario."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al resetear contraseña del usuario {Id}", id);
                throw;
            }
        }

        public async Task<UsuarioPerfilDTO> GetPerfilUsuarioAsync(int userId)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UsuarioID == userId);

                if (usuario == null)
                    throw new InvalidOperationException("Usuario no encontrado");

                // Estadísticas del mes actual
                var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var auditoriasMesActual = await _context.AuditoriasRecepcion
                    .CountAsync(a => a.UsuarioAuditorID == userId && a.FechaInicio >= inicioMes);

                var incidenciasPendientesAsignadas = await _context.Incidencias
                    .CountAsync(i => i.UsuarioResponsableID == userId &&
                                    (i.EstadoIncidencia == "Pendiente" || i.EstadoIncidencia == "EnProceso"));

                return new UsuarioPerfilDTO
                {
                    UsuarioID = usuario.UsuarioID,
                    Usuario = usuario.NombreUsuario,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email,
                    Rol = usuario.Rol,
                    FechaCreacion = usuario.FechaCreacion,
                    UltimoAcceso = usuario.UltimoAcceso,
                    AuditoriasMesActual = auditoriasMesActual,
                    IncidenciasPendientesAsignadas = incidenciasPendientesAsignadas
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfil del usuario {UserId}", userId);
                throw;
            }
        }

        public async Task<UsuarioPerfilDTO> UpdatePerfilUsuarioAsync(int userId, UpdatePerfilDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                    throw new InvalidOperationException("Usuario no encontrado");

                // Validar email único si cambió
                if (dto.Email != usuario.Email &&
                    await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.UsuarioID != userId))
                    throw new InvalidOperationException($"Ya existe un usuario con el email: {dto.Email}");

                usuario.NombreCompleto = dto.NombreCompleto;
                usuario.Email = dto.Email;
                usuario.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "ActualizarPerfil", "Usuario", userId);

                await transaction.CommitAsync();

                _logger.LogInformation("Perfil actualizado por usuario {UserId}", userId);

                return await GetPerfilUsuarioAsync(userId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al actualizar perfil del usuario {UserId}", userId);
                throw;
            }
        }

        public async Task<List<ActividadUsuarioDTO>> GetActividadUsuarioAsync(int userId, DateTime? fechaDesde, DateTime? fechaHasta, int limit)
        {
            try
            {
                var query = _context.AuditoriaLogs
                    .Where(a => a.UsuarioID == userId)
                    .AsQueryable();

                if (fechaDesde.HasValue)
                    query = query.Where(a => a.FechaHora >= fechaDesde.Value);

                if (fechaHasta.HasValue)
                    query = query.Where(a => a.FechaHora <= fechaHasta.Value);

                var actividades = await query
                    .OrderByDescending(a => a.FechaHora)
                    .Take(limit)
                    .Select(a => new ActividadUsuarioDTO
                    {
                        ActividadID = a.LogID,
                        TipoAccion = a.TipoAccion,
                        TablaAfectada = a.TablaAfectada,
                        RegistroID = a.RegistroID,
                        Descripcion = $"{a.TipoAccion} en {a.TablaAfectada}",
                        FechaHora = a.FechaHora,
                        DireccionIP = ""
                    })
                    .ToListAsync();

                return actividades;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividad del usuario {UserId}", userId);
                throw;
            }
        }

        public async Task<List<RolDTO>> GetRolesDisponiblesAsync()
        {
            try
            {
                return await Task.FromResult(new List<RolDTO>
                {
                    new RolDTO
                    {
                        Codigo = "Operador",
                        Nombre = "Operador de Recepción",
                        Descripcion = "Realiza auditorías de recepción, registra productos y detecta incidencias",
                        Permisos = new List<string> { "crear_auditorias", "registrar_productos", "subir_evidencias", "reportar_incidencias" }
                    },
                    new RolDTO
                    {
                        Codigo = "JefeLogistica",
                        Nombre = "Jefe de Logística",
                        Descripcion = "Supervisa auditorías, gestiona incidencias y genera reportes",
                        Permisos = new List<string> { "ver_todas_auditorias", "cerrar_auditorias", "gestionar_incidencias", "ver_reportes", "ver_dashboard" }
                    },
                    new RolDTO
                    {
                        Codigo = "Comprador",
                        Nombre = "Comprador",
                        Descripcion = "Recibe notificaciones de incidencias y gestiona resoluciones con proveedores",
                        Permisos = new List<string> { "ver_incidencias", "resolver_incidencias", "contactar_proveedores", "ver_reportes_proveedores" }
                    },
                    new RolDTO
                    {
                        Codigo = "Administrador",
                        Nombre = "Administrador",
                        Descripcion = "Acceso completo al sistema, gestión de usuarios y configuración",
                        Permisos = new List<string> { "acceso_total", "gestionar_usuarios", "configurar_sistema", "ver_logs", "generar_reportes_completos" }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles disponibles");
                throw;
            }
        }

        public async Task<EstadisticasUsuariosDTO> GetEstadisticasUsuariosAsync()
        {
            try
            {
                var totalUsuarios = await _context.Usuarios.CountAsync();
                var usuariosActivos = await _context.Usuarios.CountAsync(u => u.Activo);
                var usuariosInactivos = totalUsuarios - usuariosActivos;

                var usuariosPorRol = await _context.Usuarios
                    .GroupBy(u => u.Rol)
                    .Select(g => new { Rol = g.Key, Cantidad = g.Count() })
                    .ToDictionaryAsync(x => x.Rol, x => x.Cantidad);

                var hoy = DateTime.Today;
                var usuariosConectadosHoy = await _context.Usuarios
                    .CountAsync(u => u.UltimoAcceso.HasValue && u.UltimoAcceso.Value.Date == hoy);

                var ultimoUsuario = await _context.Usuarios
                    .OrderByDescending(u => u.FechaCreacion)
                    .FirstOrDefaultAsync();

                return new EstadisticasUsuariosDTO
                {
                    TotalUsuarios = totalUsuarios,
                    UsuariosActivos = usuariosActivos,
                    UsuariosInactivos = usuariosInactivos,
                    UsuariosPorRol = usuariosPorRol,
                    UsuariosConectadosHoy = usuariosConectadosHoy,
                    UltimoUsuarioCreado = ultimoUsuario?.FechaInicio
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de usuarios");
                throw;
            }
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            try
            {
                return !await _context.Usuarios.AnyAsync(u => u.NombreUsuario == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar disponibilidad de usuario");
                throw;
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            try
            {
                return !await _context.Usuarios.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar disponibilidad de email");
                throw;
            }
        }

        // Métodos privados auxiliares
        private async Task RegistrarAuditoriaAccionAsync(int userId, string accion, string tabla, int registroId)
        {
            try
            {
                var log = new AuditoriaLog
                {
                    UsuarioID = userId,
                    TipoAccion = accion,
                    TablaAfectada = tabla,
                    RegistroID = registroId,
                    FechaHora = DateTime.Now
                };

                _context.AuditoriaLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar auditoría de acción");
                // No lanzar excepción para no afectar el flujo principal
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
            var random = new byte[12];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }

            var result = new char[12];
            for (int i = 0; i < 12; i++)
            {
                result[i] = chars[random[i] % chars.Length];
            }

            return new string(result);
        }

        private UsuarioDTO MapToUsuarioDTO(Usuario usuario)
        {
            return new UsuarioDTO
            {
                UsuarioID = usuario.UsuarioID,
                Usuario = usuario.NombreUsuario,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Activo = usuario.Activo,
                UltimoAcceso = usuario.UltimoAcceso,
                FechaCreacion = usuario.FechaCreacion
            };
        }
    }
}