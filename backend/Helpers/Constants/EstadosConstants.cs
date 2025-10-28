namespace AuditoriaRecepcion.Helpers.Constants
{
    public static class EstadosConstants
    {
        // Estados de Auditoría
        public static class Auditoria
        {
            public const string Abierta = "Abierta";
            public const string Cerrada = "Cerrada";

            public static readonly string[] Todos = new[] { Abierta, Cerrada };
        }

        // Estados de Incidencia
        public static class Incidencia
        {
            public const string Pendiente = "Pendiente";
            public const string EnProceso = "EnProceso";
            public const string Resuelta = "Resuelta";
            public const string Rechazada = "Rechazada";

            public static readonly string[] Todos = new[] { Pendiente, EnProceso, Resuelta, Rechazada };
        }

        // Estados de Producto en Auditoría
        public static class EstadoProducto
        {
            public const string Bueno = "Bueno";
            public const string Dañado = "Dañado";
            public const string Defectuoso = "Defectuoso";

            public static readonly string[] Todos = new[] { Bueno, Dañado, Defectuoso };
        }

        // Tipos de Incidencia
        public static class TipoIncidencia
        {
            public const string Faltante = "Faltante";
            public const string Excedente = "Excedente";
            public const string Dañado = "Dañado";
            public const string Defectuoso = "Defectuoso";
            public const string Incorrecto = "Incorrecto";
            public const string DocumentacionIncompleta = "DocumentacionIncompleta";
            public const string Otro = "Otro";

            public static readonly string[] Todos = new[]
            {
                Faltante, Excedente, Dañado, Defectuoso, Incorrecto, DocumentacionIncompleta, Otro
            };
        }

        // Prioridades
        public static class Prioridad
        {
            public const string Baja = "Baja";
            public const string Media = "Media";
            public const string Alta = "Alta";
            public const string Critica = "Critica";

            public static readonly string[] Todas = new[] { Baja, Media, Alta, Critica };
        }
    }
}