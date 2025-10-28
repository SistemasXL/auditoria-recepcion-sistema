using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.DTOs.Proveedor;
using AuditoriaRecepcion.DTOs.Common;
using AuditoriaRecepcion.Models;
using AuditoriaRecepcion.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AuditoriaRecepcion.Services.Implementation
{
    public class ProveedorService : IProveedorService
    {
        private readonly AuditoriaRecepcionContext _context;
        private readonly ILogger<ProveedorService> _logger;

        public ProveedorService(
            AuditoriaRecepcionContext context,
            ILogger<ProveedorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedResult<ProveedorDTO>> GetProveedoresAsync(ProveedorFiltroDTO filtro)
        {
            try
            {
                var query = _context.Proveedores
                    .AsNoTracking()
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(filtro.Busqueda))
                {
                    query = query.Where(p =>
                        p.NombreProveedor.Contains(filtro.Busqueda) ||
                        p.NombreProveedor.Contains(filtro.Busqueda) ||
                        p.CUIT.Contains(filtro.Busqueda) ||
                        p.Email.Contains(filtro.Busqueda));
                }

                if (filtro.Activo.HasValue)
                    query = query.Where(p => p.Activo == filtro.Activo.Value);

                // Provincia no existe en el modelo actual
                // if (!string.IsNullOrEmpty(filtro.Provincia))
                //     query = query.Where(p => p.Provincia == filtro.Provincia);

                // Contar total
                var totalItems = await query.CountAsync();

                // Ordenar y paginar
                var items = await query
                    .OrderBy(p => p.NombreProveedor)
                    .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                    .Take(filtro.PageSize)
                    .Select(p => MapToProveedorDTO(p))
                    .ToListAsync();

                return new PaginatedResult<ProveedorDTO>(items, totalItems, filtro.PageNumber, filtro.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedores");
                throw;
            }
        }

        public async Task<ProveedorDTO> GetProveedorByIdAsync(int id)
        {
            try
            {
                var proveedor = await _context.Proveedores
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProveedorID == id);

                return proveedor != null ? MapToProveedorDTO(proveedor) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedor {Id}", id);
                throw;
            }
        }

        public async Task<ProveedorDTO> GetProveedorByCuitAsync(string cuit)
        {
            try
            {
                var proveedor = await _context.Proveedores
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.CUIT == cuit);

                return proveedor != null ? MapToProveedorDTO(proveedor) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar proveedor por CUIT {CUIT}", cuit);
                throw;
            }
        }

        public async Task<ProveedorDTO> CreateProveedorAsync(CreateProveedorDTO dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar CUIT único
                if (await _context.Proveedores.AnyAsync(p => p.CUIT == dto.CUIT))
                    throw new InvalidOperationException($"Ya existe un proveedor con el CUIT: {dto.CUIT}");

                // Validar formato CUIT
                if (!await ValidarCuitAsync(dto.CUIT))
                    throw new InvalidOperationException("CUIT inválido");

                var proveedor = new Proveedor
                {
                    NombreProveedor = dto.NombreProveedor,
                    CUIT = dto.CUIT,
                    Contacto = dto.Contacto,
                    Email = dto.Email,
                    Telefono = dto.Telefono,
                    Direccion = dto.Direccion,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                _context.Proveedores.Add(proveedor);
                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "Crear", "Proveedor", proveedor.ProveedorID);

                await transaction.CommitAsync();

                _logger.LogInformation("Proveedor creado: {CUIT} por usuario {UserId}", dto.CUIT, userId);

                return MapToProveedorDTO(proveedor);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al crear proveedor");
                throw;
            }
        }

        public async Task<ProveedorDTO> UpdateProveedorAsync(int id, UpdateProveedorDTO dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var proveedor = await _context.Proveedores.FindAsync(id);
                if (proveedor == null)
                    return null;

                proveedor.NombreProveedor = dto.NombreProveedor;
                proveedor.NombreProveedor = dto.NombreProveedor;
                proveedor.Contacto = dto.Contacto;
                proveedor.Email = dto.Email;
                proveedor.Telefono = dto.Telefono;
                proveedor.Direccion = dto.Direccion;
                proveedor.FechaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "Actualizar", "Proveedor", id);

                await transaction.CommitAsync();

                _logger.LogInformation("Proveedor actualizado: {Id} por usuario {UserId}", id, userId);

                return MapToProveedorDTO(proveedor);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al actualizar proveedor {Id}", id);
                throw;
            }
        }

        public async Task ToggleProveedorStatusAsync(int id, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var proveedor = await _context.Proveedores.FindAsync(id);
                if (proveedor == null)
                    throw new InvalidOperationException("Proveedor no encontrado");

                proveedor.Activo = !proveedor.Activo;
                await _context.SaveChangesAsync();

                // Registrar auditoría
                await RegistrarAuditoriaAccionAsync(userId, "CambiarEstado", "Proveedor", id);

                await transaction.CommitAsync();

                _logger.LogInformation("Estado de proveedor cambiado: {Id} a {Estado} por usuario {UserId}",
                    id, proveedor.Activo ? "Activo" : "Inactivo", userId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al cambiar estado del proveedor {Id}", id);
                throw;
            }
        }

        public async Task<ProveedorEstadisticasDTO> GetEstadisticasProveedorAsync(int id, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                var proveedor = await _context.Proveedores
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProveedorID == id);

                if (proveedor == null)
                    throw new InvalidOperationException("Proveedor no encontrado");

                var query = _context.AuditoriasRecepcion
                    .Include(a => a.Incidencias)
                    .Where(a => a.ProveedorID == id)
                    .AsQueryable();

                if (fechaDesde.HasValue)
                    query = query.Where(a => a.FechaInicio >= fechaDesde.Value);

                if (fechaHasta.HasValue)
                    query = query.Where(a => a.FechaInicio <= fechaHasta.Value);

                var auditorias = await query.ToListAsync();

                var totalAuditorias = auditorias.Count;
                var totalIncidencias = auditorias.Sum(a => a.Incidencias.Count);
                var porcentajeIncidencias = totalAuditorias > 0 ? (decimal)totalIncidencias / totalAuditorias * 100 : 0;

                var incidenciasPendientes = auditorias
                    .SelectMany(a => a.Incidencias)
                    .Count(i => i.EstadoIncidencia == "Pendiente" || i.EstadoIncidencia == "EnProceso");

                var incidenciasResueltas = auditorias
                    .SelectMany(a => a.Incidencias)
                    .Count(i => i.EstadoIncidencia == "Resuelta");

                var incidenciasConTiempo = auditorias
                    .SelectMany(a => a.Incidencias)
                    .Where(i => i.FechaResolucion.HasValue)
                    .ToList();

                var tiempoPromedioResolucion = incidenciasConTiempo.Any()
                    ? (decimal)incidenciasConTiempo
                        .Average(i => (i.FechaResolucion!.Value - i.FechaReporte).TotalHours)
                    : 0;

                var incidenciasPorTipo = auditorias
                    .SelectMany(a => a.Incidencias)
                    .GroupBy(i => i.TipoIncidencia)
                    .ToDictionary(g => g.Key, g => g.Count());

                return new ProveedorEstadisticasDTO
                {
                    ProveedorID = id,
                    RazonSocial = proveedor.NombreProveedor,
                    TotalAuditorias = totalAuditorias,
                    TotalIncidencias = totalIncidencias,
                    PorcentajeIncidencias = porcentajeIncidencias,
                    IncidenciasPendientes = incidenciasPendientes,
                    IncidenciasResueltas = incidenciasResueltas,
                    TiempoPromedioResolucion = tiempoPromedioResolucion,
                    UltimaRecepcion = auditorias.Any() ? auditorias.Max(a => a.FechaInicio) : null,
                    IncidenciasPorTipo = incidenciasPorTipo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas del proveedor {Id}", id);
                throw;
            }
        }

        public async Task<bool> ValidarCuitAsync(string cuit)
        {
            // Validar formato XX-XXXXXXXX-X
            var regex = new Regex(@"^\d{2}-\d{8}-\d{1}$");
            if (!regex.IsMatch(cuit))
                return false;

            // Remover guiones para validación
            var cuitNumeros = cuit.Replace("-", "");

            // Validar dígito verificador
            var multiplicadores = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            var suma = 0;

            for (int i = 0; i < 10; i++)
            {
                suma += int.Parse(cuitNumeros[i].ToString()) * multiplicadores[i];
            }

            var resto = suma % 11;
            var digitoVerificador = resto == 0 ? 0 : resto == 1 ? 9 : 11 - resto;

            return digitoVerificador == int.Parse(cuitNumeros[10].ToString());
        }

        // Métodos privados auxiliares
        private async Task RegistrarAuditoriaAccionAsync(int userId, string accion, string tabla, int registroId)
        {
            try
            {
                var log = new AuditoriaLog
                {
                    UsuarioID = userId,
                    TipoAccion = accion,
                    TablaAfectada = tabla,
                    RegistroID = registroId,
                    FechaHora = DateTime.Now
                };

                _context.AuditoriaLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar auditoría de acción");
                // No lanzar excepción para no afectar el flujo principal
            }
        }

        private ProveedorDTO MapToProveedorDTO(Proveedor proveedor)
        {
            return new ProveedorDTO
            {
                ProveedorID = proveedor.ProveedorID,
                RazonSocial = proveedor.NombreProveedor,
                NombreFantasia = proveedor.NombreProveedor,
                CUIT = proveedor.CUIT,
                Contacto = proveedor.Contacto,
                Email = proveedor.Email,
                Telefono = proveedor.Telefono,
                Direccion = proveedor.Direccion,
                Activo = proveedor.Activo,
                FechaCreacion = proveedor.FechaCreacion
            };
        }
    }
}