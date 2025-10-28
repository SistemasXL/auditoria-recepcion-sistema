# Crear archivo: .devcontainer/setup.sh
#!/bin/bash

echo "🚀 Configurando entorno de desarrollo..."

# Ir al directorio de la API
cd backend/AuditoriaRecepciónAPI

# Restaurar paquetes .NET
echo "📦 Restaurando paquetes NuGet..."
dotnet restore

# Instalar herramientas EF Core
echo "🔧 Instalando Entity Framework tools..."
dotnet tool install --global dotnet-ef

# Volver al directorio raíz
cd ../..

# Crear carpetas necesarias
echo "📁 Creando estructura de carpetas..."
mkdir -p Storage/{Evidencias,Reportes,Temp}
mkdir -p Logs

# Configurar certificados de desarrollo
echo "🔐 Configurando certificados SSL..."
cd backend/AuditoriaRecepciónAPI
dotnet dev-certs https

echo "✅ Configuración completada!"
echo ""
echo "Para ejecutar la API:"
echo "  cd backend/AuditoriaRecepciónAPI"
echo "  dotnet run"