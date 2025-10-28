namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class TopProductoIncidenciasDTO
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; }
        public string SKU { get; set; }
        public int TotalRecepciones { get; set; }
        public int TotalIncidencias { get; set; }
        public decimal PorcentajeIncidencias { get; set; }
        public Dictionary<string, int> IncidenciasPorTipo { get; set; }
    }
}