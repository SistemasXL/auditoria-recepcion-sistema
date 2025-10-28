namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class DistribucionIncidenciasDTO
    {
        public string TipoIncidencia { get; set; }
        public int Cantidad { get; set; }
        public decimal Porcentaje { get; set; }
        public string Color { get; set; } // Para gráficos
    }
}