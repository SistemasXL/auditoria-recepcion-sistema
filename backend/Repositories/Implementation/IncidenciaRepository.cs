using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuditoriaRecepcion.Repositories.Implementation
{
    public class IncidenciaRepository : BaseRepository<Incidencia>, IIncidenciaRepository
    {
        public IncidenciaRepository(AuditoriaRecepcionContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Incidencia>> GetByAuditoriaAsync(int auditoriaId)
        {
            return await _dbSet
                .Where(i => i.AuditoriaID == auditoriaId)
                .OrderByDescending(i => i.FechaReporte)
                .ToListAsync();
        }

        public async Task<IEnumerable<Incidencia>> GetByProveedorAsync(
            int proveedorId, 
            DateTime? fechaDesde, 
            DateTime? fechaHasta)
        {
            var query = _dbSet
                .Include(i => i.Auditoria)
                .Where(i => i.Auditoria.ProveedorID == proveedorId);

            if (fechaDesde.HasValue)
                query = query.Where(i => i.FechaReporte >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(i => i.FechaReporte <= fechaHasta.Value);

            return await query
                .OrderByDescending(i => i.FechaReporte)
                .ToListAsync();
        }

        public async Task<IEnumerable<Incidencia>> GetByProductoAsync(
            int productoId, 
            DateTime? fechaDesde, 
            DateTime? fechaHasta)
        {
            // ProductoID no existe en el modelo Incidencia
            // Este método necesita ser reimplementado o eliminado
            return await _dbSet
                .Include(i => i.Auditoria)
                .Where(i => i.AuditoriaID > 0) // placeholder
                .OrderByDescending(i => i.FechaReporte)
                .ToListAsync();
        }

        public async Task<IEnumerable<Incidencia>> GetByUsuarioAsignadoAsync(int usuarioId, string estado = null)
        {
            var query = _dbSet
                .Include(i => i.Auditoria)
                .Where(i => i.UsuarioResponsableID == usuarioId);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(i => i.EstadoIncidencia == estado);

            return await query
                .OrderByDescending(i => i.Prioridad)
                .ThenBy(i => i.FechaReporte)
                .ToListAsync();
        }

        public async Task<IEnumerable<Incidencia>> GetPendientesAsync()
        {
            return await _dbSet
                .Include(i => i.Auditoria)
                .Where(i => i.EstadoIncidencia == "Pendiente" || i.EstadoIncidencia == "EnProceso")
                .OrderByDescending(i => i.Prioridad)
                .ThenBy(i => i.FechaReporte)
                .ToListAsync();
        }

        public async Task<IEnumerable<Incidencia>> GetPorVencerAsync(int horasLimite = 48)
        {
            var fechaLimite = DateTime.Now.AddHours(-horasLimite);

            return await _dbSet
                .Include(i => i.Auditoria)
                .Where(i => 
                    (i.EstadoIncidencia == "Pendiente" || i.EstadoIncidencia == "EnProceso") &&
                    i.FechaReporte <= fechaLimite)
                .OrderBy(i => i.FechaReporte)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetDistribucionPorTipoAsync(
            DateTime? fechaDesde, 
            DateTime? fechaHasta)
        {
            var query = _dbSet.AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(i => i.FechaReporte >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(i => i.FechaReporte <= fechaHasta.Value);

            return await query
                .GroupBy(i => i.TipoIncidencia)
                .Select(g => new { Tipo = g.Key, Cantidad = g.Count() })
                .ToDictionaryAsync(x => x.Tipo, x => x.Cantidad);
        }

        public async Task<Dictionary<string, int>> GetDistribucionPorEstadoAsync(
            DateTime? fechaDesde, 
            DateTime? fechaHasta)
        {
            var query = _dbSet.AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(i => i.FechaReporte >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(i => i.FechaReporte <= fechaHasta.Value);

            return await query
                .GroupBy(i => i.EstadoIncidencia)
                .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                .ToDictionaryAsync(x => x.Estado, x => x.Cantidad);
        }
    }
}