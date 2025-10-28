namespace AuditoriaRecepcion.Helpers.Constants
{
    public static class ErrorMessages
    {
        // Errores de autenticación
        public const string CredencialesInvalidas = "Usuario o contraseña incorrectos";
        public const string TokenExpirado = "El token de sesión ha expirado";
        public const string AccesoNoAutorizado = "No tiene permisos para realizar esta acción";
        public const string SesionInvalida = "Sesión inválida o expirada";

        // Errores de validación
        public const string CampoRequerido = "El campo {0} es requerido";
        public const string FormatoInvalido = "El formato del campo {0} es inválido";
        public const string ValorDuplicado = "Ya existe un registro con el valor {0}";
        public const string RangoInvalido = "El valor debe estar entre {0} y {1}";

        // Errores de negocio
        public const string AuditoriaCerrada = "No se pueden realizar cambios en una auditoría cerrada";
        public const string ProductoNoEncontrado = "Producto no encontrado";
        public const string ProveedorNoEncontrado = "Proveedor no encontrado";
        public const string IncidenciasNoResueltas = "Existen incidencias pendientes de resolución";
        public const string ArchivoNoEncontrado = "Archivo no encontrado";
        public const string ArchivoInvalido = "El archivo no es válido o está corrupto";

        // Errores de sistema
        public const string ErrorInterno = "Ha ocurrido un error interno. Por favor, contacte al administrador";
        public const string ErrorBaseDatos = "Error al acceder a la base de datos";
        public const string ErrorServicio = "El servicio no está disponible en este momento";
    }
}