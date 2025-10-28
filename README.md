# 🏭 Sistema de Auditoría de Recepción

Sistema completo para gestión y auditoría de recepciones de mercadería con seguimiento de incidencias.

## 🚀 Desarrollo en GitHub Codespace

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/SistemasXL/auditoria-recepcion-sistema)

### Inicio Rápido

1. **Abrir Codespace**: Clic en el botón de arriba o ve a tu repositorio → Code → Codespaces → Create
2. **Esperar configuración**: ~2-3 minutos (automático)
3. **Ejecutar API**:
   ```bash
   cd backend/AuditoriaRecepciónAPI
   dotnet run
   ```

### URLs de Acceso
- **API**: Puerto 5000 de tu Codespace
- **Swagger**: `{URL_CODESPACE}:5000/swagger`
- **Health Check**: `{URL_CODESPACE}:5000/health`

## 🏗️ Arquitectura del Proyecto

```
📁 backend/
├── 🎯 AuditoriaRecepciónAPI/     # API Principal
│   ├── Controllers/              # Endpoints REST
│   ├── Services/                 # Lógica de negocio
│   ├── Models/                   # Entidades de base de datos
│   ├── Data/                     # DbContext y configuraciones
│   └── Helpers/                  # Utilidades y constantes
├── 📊 DTOs/                      # Data Transfer Objects
└── 🧪 Tests/                     # Pruebas unitarias (futuro)
```

## 🛠️ Stack Tecnológico

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM y manejo de datos
- **SQL Server** - Base de datos
- **JWT** - Autenticación y autorización
- **Serilog** - Sistema de logging
- **Swagger/OpenAPI** - Documentación de API

## 📋 Funcionalidades Principales

- ✅ **Gestión de Auditorías**: Control completo del proceso de recepción
- ✅ **Proveedores y Productos**: Catálogo maestro con validaciones
- ✅ **Control de Incidencias**: Registro y seguimiento de problemas
- ✅ **Dashboard y KPIs**: Métricas y análisis en tiempo real
- ✅ **Sistema de Reportes**: Generación de informes personalizados
- ✅ **Evidencias Digitales**: Manejo de fotos, videos y documentos
- ✅ **Notificaciones**: Sistema de alertas por email

## 🚀 Desarrollo Local

```bash
# Clonar repositorio
git clone https://github.com/SistemasXL/auditoria-recepcion-sistema.git

# Ir al directorio del API
cd auditoria-recepcion-sistema/backend/AuditoriaRecepciónAPI

# Restaurar dependencias
dotnet restore

# Ejecutar aplicación
dotnet run
```

---
**Desarrollado con ❤️ para optimizar procesos de recepción**