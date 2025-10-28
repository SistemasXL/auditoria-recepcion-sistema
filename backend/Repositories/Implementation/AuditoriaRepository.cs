using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuditoriaRecepcion.Repositories.Implementation
{
    public class AuditoriaRepository : BaseRepository<Models.AuditoriaRecepcion>, IAuditoriaRepository
    {
        public AuditoriaRepository(AuditoriaRecepcionContext context) : base(context)
        {
        }

        public async Task<Models.AuditoriaRecepcion> GetAuditoriaCompletaAsync(int id)
        {
            return await _dbSet
                .Include(a => a.Proveedor)
                .Include(a => a.UsuarioAuditor)
                .Include(a => a.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(a => a.Incidencias)
                    .ThenInclude(i => i.UsuarioReporta)
                .Include(a => a.Incidencias)
                    .ThenInclude(i => i.UsuarioResponsable)
                .Include(a => a.Evidencias)
                .FirstOrDefaultAsync(a => a.AuditoriaID == id);
        }

        public async Task<IEnumerable<Models.AuditoriaRecepcion>> GetAuditoriasByProveedorAsync(
            int proveedorId, 
            DateTime? fechaDesde, 
            DateTime? fechaHasta)
        {
            var query = _dbSet
                .Include(a => a.Proveedor)
                .Where(a => a.ProveedorID == proveedorId);

            if (fechaDesde.HasValue)
                query = query.Where(a => a.FechaInicio >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(a => a.FechaInicio <= fechaHasta.Value);

            return await query
                .OrderByDescending(a => a.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.AuditoriaRecepcion>> GetAuditoriasAbiertasAsync()
        {
            return await _dbSet
                .Include(a => a.Proveedor)
                .Include(a => a.UsuarioAuditor)
                .Where(a => a.EstadoAuditoria == "En Proceso")
                .OrderByDescending(a => a.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Models.AuditoriaRecepcion>> GetAuditoriasVencidasAsync(int diasVencimiento = 7)
        {
            var fechaLimite = DateTime.Now.AddDays(-diasVencimiento);

            return await _dbSet
                .Include(a => a.Proveedor)
                .Include(a => a.UsuarioAuditor)
                .Where(a => a.EstadoAuditoria == "En Proceso" && a.FechaInicio <= fechaLimite)
                .OrderBy(a => a.FechaInicio)
                .ToListAsync();
        }

        public async Task<string> GenerarNumeroAuditoriaAsync()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month.ToString("D2");

            var ultimaAuditoria = await _dbSet
                .Where(a => a.NumeroAuditoria.StartsWith($"AUD-{year}{month}"))
                .OrderByDescending(a => a.NumeroAuditoria)
                .FirstOrDefaultAsync();

            int consecutivo = 1;
            if (ultimaAuditoria != null)
            {
                var partes = ultimaAuditoria.NumeroAuditoria.Split('-');
                if (partes.Length == 3 && int.TryParse(partes[2], out int numero))
                {
                    consecutivo = numero + 1;
                }
            }

            return $"AUD-{year}{month}-{consecutivo:D5}";
        }

        public async Task<bool> ExisteOrdenCompraAsync(string numeroOrdenCompra)
        {
            return await _dbSet.AnyAsync(a => a.NumeroOrdenCompra == numeroOrdenCompra);
        }
    }
}