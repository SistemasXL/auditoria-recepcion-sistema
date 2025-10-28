namespace AuditoriaRecepcion.Helpers.Constants
{
    public static class RolesConstants
    {
        public const string Administrador = "Administrador";
        public const string JefeLogistica = "JefeLogistica";
        public const string Operador = "Operador";
        public const string Comprador = "Comprador";

        public static readonly string[] TodosLosRoles = new[]
        {
            Administrador,
            JefeLogistica,
            Operador,
            Comprador
        };

        public static readonly string[] RolesAdministrativos = new[]
        {
            Administrador,
            JefeLogistica
        };

        public static readonly string[] RolesOperativos = new[]
        {
            Operador,
            JefeLogistica
        };
    }
}