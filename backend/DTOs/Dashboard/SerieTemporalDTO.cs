namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class SerieTemporalDTO
    {
        public DateTime Fecha { get; set; }
        public string Etiqueta { get; set; }
        public decimal Valor { get; set; }
        public Dictionary<string, decimal> ValoresAdicionales { get; set; }
    }
}