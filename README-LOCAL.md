# 🏭 Sistema de Auditoría de Recepción - Guía de Instalación Local

> Guía completa para ejecutar el Sistema de Auditoría de Recepción en tu computadora local

## 📋 Tabla de Contenidos

- [Requisitos Previos](#-requisitos-previos)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Ejecución](#-ejecución)
- [Acceso a la Aplicación](#-acceso-a-la-aplicación)
- [Solución de Problemas](#-solución-de-problemas)

---

## 🔧 Requisitos Previos

### Backend (.NET API)
- ✅ **.NET 8 SDK** - [Descargar aquí](https://dotnet.microsoft.com/download/dotnet/8.0)
- ✅ **SQL Server 2019+** o **SQL Server Express** - [Descargar aquí](https://www.microsoft.com/sql-server/sql-server-downloads)
  - Alternativa: **SQL Server LocalDB** (viene con Visual Studio)
- ✅ **Visual Studio Code** con extensión C# - [Descargar aquí](https://code.visualstudio.com/)
  - O **Visual Studio 2022** Community - [Descargar aquí](https://visualstudio.microsoft.com/)

### Frontend (React Web App)
- ✅ **Node.js 18+** - [Descargar aquí](https://nodejs.org/)
- ✅ **npm 9+** (se instala con Node.js)

### Herramientas Opcionales
- 🔹 **Git** - Para clonar el repositorio
- 🔹 **SQL Server Management Studio (SSMS)** - Para gestionar la base de datos
- 🔹 **Postman** - Para probar la API

---

## 📥 Instalación

### Paso 1: Clonar el Repositorio

```bash
# Clonar desde GitHub
git clone https://github.com/SistemasXL/auditoria-recepcion-sistema.git

# Entrar al directorio
cd auditoria-recepcion-sistema
```

### Paso 2: Instalar .NET 8 SDK

```bash
# Verificar si ya está instalado
dotnet --version

# Debería mostrar: 8.0.x
```

Si no está instalado, descargarlo de: https://dotnet.microsoft.com/download/dotnet/8.0

### Paso 3: Instalar SQL Server

#### Opción A: SQL Server Express (Recomendado para desarrollo)

1. Descargar SQL Server Express: https://www.microsoft.com/sql-server/sql-server-downloads
2. Ejecutar el instalador
3. Seleccionar "Basic installation"
4. Anotar el nombre del servidor (ejemplo: `localhost\SQLEXPRESS`)

#### Opción B: SQL Server LocalDB (Más simple)

```bash
# Verificar si ya está instalado
sqllocaldb info

# Si no aparece "MSSQLLocalDB", crear instancia
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

### Paso 4: Crear la Base de Datos

#### Usando SQL Server Management Studio (SSMS):

```sql
-- Conectarse al servidor y ejecutar:
CREATE DATABASE AuditoriaRecepcionDB;
GO
```

#### Usando línea de comandos:

```bash
# Para SQL Server Express
sqlcmd -S localhost\SQLEXPRESS -Q "CREATE DATABASE AuditoriaRecepcionDB"

# Para LocalDB
sqlcmd -S (localdb)\MSSQLLocalDB -Q "CREATE DATABASE AuditoriaRecepcionDB"
```

### Paso 5: Instalar Dependencias del Backend

```bash
cd backend
dotnet restore
dotnet build
```

### Paso 6: Instalar Dependencias del Frontend

```bash
cd auditor-recepcion-web
npm install
```

---

## ⚙️ Configuración

### Configurar Backend

#### 1. Crear archivo de configuración de desarrollo

Crear el archivo: `backend/appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AuditoriaRecepcionDB;Integrated Security=true;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Key": "development-secret-key-minimum-32-characters-for-jwt-security-token",
    "Issuer": "AuditoriaRecepcionAPI",
    "Audience": "AuditoriaRecepcionWeb"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "http://localhost:4200"
    ]
  },
  "Application": {
    "Name": "API Auditoría Recepción",
    "Version": "1.0.0",
    "Environment": "Development",
    "EnableSwagger": true,
    "AutoMigrate": false
  },
  "RateLimiting": {
    "EnableRateLimiting": false,
    "PermitLimit": 100,
    "Window": 60,
    "QueueLimit": 10
  },
  "Performance": {
    "EnablePerformanceMonitoring": true
  }
}
```

#### 2. Ajustar ConnectionString según tu instalación

**Para LocalDB (por defecto):**
```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AuditoriaRecepcionDB;Integrated Security=true;TrustServerCertificate=true;"
```

**Para SQL Server Express:**
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=AuditoriaRecepcionDB;Integrated Security=true;TrustServerCertificate=true;"
```

**Para SQL Server con usuario/contraseña:**
```json
"DefaultConnection": "Server=localhost;Database=AuditoriaRecepcionDB;User Id=sa;Password=TuPasswordSeguro;TrustServerCertificate=true;"
```

#### 3. Crear carpetas necesarias

```bash
# Desde el directorio backend
cd backend
mkdir Storage
mkdir Storage\Evidencias
mkdir Storage\Reportes
mkdir Storage\Temp
mkdir Logs
```

O en PowerShell:
```powershell
New-Item -ItemType Directory -Force -Path Storage\Evidencias
New-Item -ItemType Directory -Force -Path Storage\Reportes
New-Item -ItemType Directory -Force -Path Storage\Temp
New-Item -ItemType Directory -Force -Path Logs
```

#### 4. Aplicar Migraciones de Base de Datos

```bash
# Instalar herramienta EF Core (solo una vez)
dotnet tool install --global dotnet-ef

# Verificar instalación
dotnet ef --version

# Crear migración inicial (si no existe)
cd backend
dotnet ef migrations add InitialCreate

# Aplicar migraciones a la base de datos
dotnet ef database update
```

### Configurar Frontend

#### 1. Crear archivo de configuración

Crear el archivo: `auditor-recepcion-web/.env`

```env
VITE_API_URL=https://localhost:5001
VITE_API_TIMEOUT=30000
```

#### 2. Verificar puerto de desarrollo

Editar `auditor-recepcion-web/vite.config.ts` para asegurar el puerto:

```typescript
export default defineConfig({
  server: {
    port: 5173,
    open: true
  }
})
```

---

## 🚀 Ejecución

### Opción 1: Ejecutar Backend y Frontend por separado

#### Iniciar Backend (Terminal 1)

```bash
# Navegar al backend
cd backend

# Ejecutar con hot reload (recomendado)
dotnet watch run

# O sin hot reload
dotnet run
```

**Salida esperada:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
Application started. Press Ctrl+C to shut down.
```

#### Iniciar Frontend (Terminal 2)

```bash
# En una nueva terminal
cd auditor-recepcion-web

# Ejecutar servidor de desarrollo
npm run dev
```

**Salida esperada:**
```
  VITE v5.0.8  ready in 1234 ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
  ➜  press h to show help
```

### Opción 2: Script de inicio automático

Crear archivo `start-dev.bat` en la raíz del proyecto:

```batch
@echo off
echo Iniciando Sistema de Auditoría de Recepción...

echo.
echo [1/2] Iniciando Backend...
start cmd /k "cd backend && dotnet watch run"

timeout /t 10

echo [2/2] Iniciando Frontend...
start cmd /k "cd auditor-recepcion-web && npm run dev"

echo.
echo ✓ Sistema iniciado correctamente
echo Backend: https://localhost:5001
echo Frontend: http://localhost:5173
pause
```

Ejecutar:
```bash
start-dev.bat
```

---

## 🌐 Acceso a la Aplicación

### URLs de la API (Backend)

| Recurso | URL | Descripción |
|---------|-----|-------------|
| **🌐 API Base** | `https://localhost:5001` | Endpoint principal |
| **📚 Swagger UI** | `https://localhost:5001/swagger` | Documentación interactiva |
| **💚 Health Check** | `https://localhost:5001/api/health` | Estado del sistema |
| **ℹ️ Info** | `https://localhost:5001/` | Información general |

### URLs de la Aplicación Web (Frontend)

| Página | URL | Descripción |
|--------|-----|-------------|
| **🏠 Inicio** | `http://localhost:5173` | Página principal |
| **🔐 Login** | `http://localhost:5173/login` | Inicio de sesión |
| **📊 Dashboard** | `http://localhost:5173/dashboard` | Panel de control |
| **📋 Auditorías** | `http://localhost:5173/auditorias` | Lista de auditorías |

### 🔑 Credenciales de Prueba

Si la base de datos se inicializa con datos de prueba:

```
Usuario: admin
Contraseña: Admin123!
```

---

## 🧪 Probar la API

### Opción 1: Usando Swagger UI (Recomendado)

1. Abrir navegador en: `https://localhost:5001/swagger`
2. Expandir el endpoint `/api/auth/login`
3. Clic en "Try it out"
4. Ingresar credenciales:
   ```json
   {
     "username": "admin",
     "password": "Admin123!"
   }
   ```
5. Clic en "Execute"
6. Copiar el `token` de la respuesta
7. Clic en el botón "Authorize" (🔒 arriba a la derecha)
8. Ingresar: `Bearer {token-copiado}`
9. Ahora puedes probar todos los endpoints

### Opción 2: Usando cURL

```bash
# 1. Login y obtener token
curl -X POST https://localhost:5001/api/auth/login ^
  -H "Content-Type: application/json" ^
  -d "{\"username\":\"admin\",\"password\":\"Admin123!\"}"

# 2. Usar el token para obtener auditorías
curl -X GET https://localhost:5001/api/auditoria-recepcion ^
  -H "Authorization: Bearer {tu-token-aqui}"
```

### Opción 3: Usando Postman

1. Crear nueva colección "Auditoría Recepción"
2. Crear request POST `https://localhost:5001/api/auth/login`
3. Body → raw → JSON:
   ```json
   {
     "username": "admin",
     "password": "Admin123!"
   }
   ```
4. Enviar y copiar el token
5. Crear request GET `https://localhost:5001/api/auditoria-recepcion`
6. Headers → Authorization: `Bearer {token}`

---

## 🐛 Solución de Problemas

### ❌ Error: "Cannot connect to SQL Server"

**Problema:** La API no puede conectarse a la base de datos.

**Soluciones:**

1. **Verificar que SQL Server esté corriendo:**
   ```bash
   # Verificar servicios
   sc query MSSQL$SQLEXPRESS
   
   # O para LocalDB
   sqllocaldb info MSSQLLocalDB
   ```

2. **Iniciar SQL Server si está detenido:**
   ```bash
   # SQL Server Express
   net start MSSQL$SQLEXPRESS
   
   # LocalDB
   sqllocaldb start MSSQLLocalDB
   ```

3. **Verificar ConnectionString en `appsettings.Development.json`**

4. **Probar conexión manualmente:**
   ```bash
   sqlcmd -S (localdb)\MSSQLLocalDB -Q "SELECT @@VERSION"
   ```

### ❌ Error: "Port 5000 or 5001 is already in use"

**Problema:** El puerto está siendo usado por otra aplicación.

**Soluciones:**

1. **Encontrar qué proceso usa el puerto:**
   ```bash
   netstat -ano | findstr :5001
   ```

2. **Terminar el proceso:**
   ```bash
   taskkill /PID <numero-pid> /F
   ```

3. **Cambiar el puerto en `backend/Properties/launchSettings.json`:**
   ```json
   "applicationUrl": "https://localhost:7001;http://localhost:7000"
   ```

### ❌ Error: "Certificate not trusted" o SSL error

**Problema:** El certificado HTTPS de desarrollo no es confiable.

**Solución:**

```bash
# Limpiar certificados anteriores
dotnet dev-certs https --clean

# Crear y confiar en nuevo certificado
dotnet dev-certs https --trust

# Reiniciar la aplicación
```

### ❌ Error: "npm install fails" o "Cannot find module"

**Problema:** Problemas con las dependencias de Node.js.

**Soluciones:**

```bash
cd auditor-recepcion-web

# 1. Limpiar caché
npm cache clean --force

# 2. Eliminar node_modules
rmdir /s /q node_modules
del package-lock.json

# 3. Reinstalar
npm install

# 4. Si persiste, actualizar npm
npm install -g npm@latest
```

### ❌ Error: "CORS policy" en el navegador

**Problema:** El frontend no puede comunicarse con el backend por CORS.

**Solución:**

1. Verificar que la URL del frontend esté en `appsettings.Development.json`:
   ```json
   "Cors": {
     "AllowedOrigins": [
       "http://localhost:5173"
     ]
   }
   ```

2. Reiniciar el backend después de cambiar la configuración.

### ❌ Error: "dotnet ef command not found"

**Problema:** Las herramientas de Entity Framework no están instaladas.

**Solución:**

```bash
# Instalar herramientas globalmente
dotnet tool install --global dotnet-ef

# Si ya está instalado, actualizar
dotnet tool update --global dotnet-ef

# Verificar instalación
dotnet ef --version
```

### ❌ Error: "No migrations found"

**Problema:** No se han creado las migraciones de la base de datos.

**Solución:**

```bash
cd backend

# Crear migración inicial
dotnet ef migrations add InitialCreate

# Aplicar a la base de datos
dotnet ef database update
```

### ❌ Error: Frontend no carga o pantalla en blanco

**Problemas comunes:**

1. **Backend no está corriendo:**
   - Verificar que `https://localhost:5001` responda

2. **URL incorrecta en .env:**
   ```env
   VITE_API_URL=https://localhost:5001
   ```

3. **Limpiar caché del navegador:**
   - Ctrl + Shift + Delete → Limpiar caché
   - O abrir en modo incógnito

---

## 📚 Comandos Útiles

### Backend

```bash
# Ejecutar con hot reload
dotnet watch run

# Compilar sin ejecutar
dotnet build

# Limpiar compilación
dotnet clean

# Ejecutar tests
dotnet test

# Ver información de .NET
dotnet --info

# Listar migraciones
dotnet ef migrations list

# Eliminar última migración
dotnet ef migrations remove

# Actualizar base de datos a migración específica
dotnet ef database update <NombreMigracion>

# Generar script SQL de migraciones
dotnet ef migrations script
```

### Frontend

```bash
# Ejecutar en modo desarrollo
npm run dev

# Compilar para producción
npm run build

# Previsualizar build de producción
npm run preview

# Linter
npm run lint

# Actualizar dependencias
npm update

# Ver dependencias desactualizadas
npm outdated
```

---

## 📖 Documentación Adicional

### Estructura del Proyecto

```
📁 auditoria-recepcion-sistema/
├── 📁 backend/                    # Backend .NET
│   ├── 📁 Controllers/            # Controladores de API
│   ├── 📁 Services/               # Lógica de negocio
│   │   ├── Implementation/
│   │   └── Interfaces/
│   ├── 📁 Models/                 # Modelos de datos
│   │   └── Entities/
│   ├── 📁 Data/                   # DbContext
│   ├── 📁 DTOs/                   # Data Transfer Objects
│   ├── 📁 Repositories/           # Repositorios
│   ├── 📁 Helpers/                # Utilidades
│   ├── 📁 Middleware/             # Middleware personalizado
│   ├── 📁 Storage/                # Almacenamiento de archivos
│   ├── 📁 Logs/                   # Archivos de log
│   ├── 📄 Program.cs              # Configuración principal
│   ├── 📄 appsettings.json        # Configuración base
│   └── 📄 appsettings.Development.json # Config desarrollo
│
└── 📁 auditor-recepcion-web/     # Frontend React
    ├── 📁 src/
    │   ├── 📁 components/         # Componentes reutilizables
    │   ├── 📁 pages/              # Páginas de la aplicación
    │   ├── 📁 services/           # Servicios API
    │   ├── 📁 store/              # Estado global (Zustand)
    │   ├── 📁 hooks/              # Custom hooks
    │   ├── 📁 utils/              # Utilidades
    │   ├── 📁 types/              # TypeScript types
    │   └── 📁 styles/             # Estilos globales
    ├── 📁 public/                 # Archivos estáticos
    ├── 📄 vite.config.ts          # Configuración Vite
    ├── 📄 package.json            # Dependencias
    └── 📄 .env                    # Variables de entorno
```

### Tecnologías Utilizadas

**Backend:**
- .NET 8
- Entity Framework Core
- SQL Server
- JWT Authentication
- Serilog
- Swagger/OpenAPI

**Frontend:**
- React 18
- TypeScript
- Vite
- Material-UI (MUI)
- Zustand (State Management)
- Axios
- React Router
- React Hook Form
- Recharts (Gráficos)

---

## 🤝 Soporte

Si tienes problemas no listados aquí:

1. Revisar los logs en `backend/Logs/`
2. Revisar la consola del navegador (F12)
3. Verificar que todos los requisitos estén instalados
4. Revisar la documentación de Swagger: `https://localhost:5001/swagger`

---

**🎉 ¡Listo! Ahora puedes usar el Sistema de Auditoría de Recepción en tu computadora local.**

---

📅 Última actualización: Noviembre 2025  
📧 Soporte: desarrollo@sistemasxl.com  
🌐 GitHub: https://github.com/SistemasXL/auditoria-recepcion-sistema
