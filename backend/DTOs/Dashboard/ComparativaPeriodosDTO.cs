namespace AuditoriaRecepcion.DTOs.Dashboard
{
    public class ComparativaPeriodosDTO
    {
        public PeriodoComparativoDTO PeriodoActual { get; set; }
        public PeriodoComparativoDTO PeriodoAnterior { get; set; }
        public Dictionary<string, decimal> Diferencias { get; set; }
        public Dictionary<string, decimal> PorcentajesCambio { get; set; }
    }
    
    public class PeriodoComparativoDTO
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TotalAuditorias { get; set; }
        public int TotalIncidencias { get; set; }
        public decimal TasaIncidencias { get; set; }
        public decimal TiempoPromedioResolucion { get; set; }
        public Dictionary<string, int> MetricasAdicionales { get; set; }
    }
}