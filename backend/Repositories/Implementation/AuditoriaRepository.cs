using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuditoriaRecepcion.Repositories.Implementation
{
    public class AuditoriaRepository : BaseRepository<Auditoria>, IAuditoriaRepository
    {
        public AuditoriaRepository(AuditoriaRecepcionContext context) : base(context)
        {
        }

        public async Task<Auditoria> GetAuditoriaCompletaAsync(int id)
        {
            return await _dbSet
                .Include(a => a.Proveedor)
                .Include(a => a.UsuarioCreacion)
                .Include(a => a.UsuarioModificacion)
                .Include(a => a.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(a => a.Incidencias)
                    .ThenInclude(i => i.UsuarioReporto)
                .Include(a => a.Incidencias)
                    .ThenInclude(i => i.UsuarioAsignado)
                .Include(a => a.Evidencias)
                .FirstOrDefaultAsync(a => a.AuditoriaID == id);
        }

        public async Task<IEnumerable<Auditoria>> GetAuditoriasByProveedorAsync(
            int proveedorId, 
            DateTime? fechaDesde, 
            DateTime? fechaHasta)
        {
            var query = _dbSet
                .Include(a => a.Proveedor)
                .Where(a => a.ProveedorID == proveedorId);

            if (fechaDesde.HasValue)
                query = query.Where(a => a.FechaRecepcion >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(a => a.FechaRecepcion <= fechaHasta.Value);

            return await query
                .OrderByDescending(a => a.FechaRecepcion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auditoria>> GetAuditoriasAbiertasAsync()
        {
            return await _dbSet
                .Include(a => a.Proveedor)
                .Include(a => a.UsuarioCreacion)
                .Where(a => a.Estado == "Abierta")
                .OrderByDescending(a => a.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auditoria>> GetAuditoriasVencidasAsync(int diasVencimiento = 7)
        {
            var fechaLimite = DateTime.Now.AddDays(-diasVencimiento);

            return await _dbSet
                .Include(a => a.Proveedor)
                .Include(a => a.UsuarioCreacion)
                .Where(a => a.Estado == "Abierta" && a.FechaCreacion <= fechaLimite)
                .OrderBy(a => a.FechaCreacion)
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