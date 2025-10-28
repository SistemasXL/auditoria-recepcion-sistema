using AuditoriaRecepcion.Data;
using AuditoriaRecepcion.DTOs.Reporte;
using AuditoriaRecepcion.DTOs.Common;
using AuditoriaRecepcion.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AuditoriaRecepcion.Services.Implementation
{
    public class ReporteService : IReporteService
    {
        private readonly AuditoriaRecepcionContext _context;
        private readonly ILogger<ReporteService> _logger;

        public ReporteService(
            AuditoriaRecepcionContext context,
            ILogger<ReporteService> logger)
        {
            _context = context;
            _logger = logger;
            
            // Configurar QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerarReporteAuditoriaPDFAsync(int auditoriaId)
        {
            try
            {
                var auditoria = await _context.AuditoriasRecepcion
                    .Include(a => a.Proveedor)
                    .Include(a => a.UsuarioAuditor)
                    .Include(a => a.Detalles)
                        .ThenInclude(d => d.Producto)
                    .Include(a => a.Incidencias)
                    .FirstOrDefaultAsync(a => a.AuditoriaID == auditoriaId);

                if (auditoria == null)
                    return null;

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(11));

                        page.Header()
                            .Text($"Auditoría de Recepción - {auditoria.NumeroAuditoria}")
                            .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                column.Spacing(5);

                                // Información general
                                column.Item().Text("Información General").SemiBold().FontSize(14);
                                column.Item().LineHorizontal(1);
                                column.Item().Text($"Fecha Recepción: {auditoria.FechaInicio:dd/MM/yyyy}");
                                column.Item().Text($"Proveedor: {auditoria.Proveedor.RazonSocial}");
                                column.Item().Text($"Orden de Compra: {auditoria.OrdenCompraID}");
                                column.Item().Text($"Estado: {auditoria.EstadoAuditoria}");

                                // Detalles de productos (simplificado)
                                column.Item().PaddingTop(20).Text("Productos Recibidos").SemiBold().FontSize(14);
                                column.Item().LineHorizontal(1);
                                
                                // TODO: Agregar tabla de productos con QuestPDF

                                // Incidencias
                                if (auditoria.Incidencias.Any())
                                {
                                    column.Item().PaddingTop(20).Text("Incidencias Detectadas").SemiBold().FontSize(14);
                                    column.Item().LineHorizontal(1);
                                    
                                    foreach (var incidencia in auditoria.Incidencias)
                                    {
                                        column.Item().Text($"• {incidencia.TipoIncidencia}: {incidencia.Descripcion}");
                                    }
                                }
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Generado el: ");
                                x.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}");
                            });
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte PDF de auditoría {Id}", auditoriaId);
                throw;
            }
        }

        public async Task<byte[]> GenerarReporteAuditoriaExcelAsync(int auditoriaId)
        {
            try
            {
                var auditoria = await _context.AuditoriasRecepcion
                    .Include(a => a.Proveedor)
                    .Include(a => a.Detalles)
                        .ThenInclude(d => d.Producto)
                    .Include(a => a.Incidencias)
                    .FirstOrDefaultAsync(a => a.AuditoriaID == auditoriaId);

                if (auditoria == null)
                    return null;

                using var workbook = new XLWorkbook();
                
                // Hoja 1: Información General
                var wsGeneral = workbook.Worksheets.Add("Información General");
                wsGeneral.Cell(1, 1).Value = "Número Auditoría";
                wsGeneral.Cell(1, 2).Value = auditoria.NumeroAuditoria;
                wsGeneral.Cell(2, 1).Value = "Fecha Recepción";
                wsGeneral.Cell(2, 2).Value = auditoria.FechaInicio;
                wsGeneral.Cell(3, 1).Value = "Proveedor";
                wsGeneral.Cell(3, 2).Value = auditoria.Proveedor.RazonSocial;
                wsGeneral.Cell(4, 1).Value = "Orden de Compra";
                wsGeneral.Cell(4, 2).Value = auditoria.OrdenCompraID;
                wsGeneral.Cell(5, 1).Value = "Estado";
                wsGeneral.Cell(5, 2).Value = auditoria.EstadoAuditoria;

                // Hoja 2: Productos
                var wsProductos = workbook.Worksheets.Add("Productos");
                wsProductos.Cell(1, 1).Value = "SKU";
                wsProductos.Cell(1, 2).Value = "Producto";
                wsProductos.Cell(1, 3).Value = "Cant. Esperada";
                wsProductos.Cell(1, 4).Value = "Cant. Recibida";
                wsProductos.Cell(1, 5).Value = "Diferencia";
                wsProductos.Cell(1, 6).Value = "Estado";

                int row = 2;
                foreach (var detalle in auditoria.Detalles)
                {
                    wsProductos.Cell(row, 1).Value = detalle.Producto.SKU;
                    wsProductos.Cell(row, 2).Value = detalle.Producto.Nombre;
                    wsProductos.Cell(row, 3).Value = detalle.CantidadEsperada;
                    wsProductos.Cell(row, 4).Value = detalle.CantidadRecibida;
                    wsProductos.Cell(row, 5).Value = detalle.CantidadEsperada - detalle.CantidadRecibida;
                    wsProductos.Cell(row, 6).Value = detalle.EstadoItem;
                    row++;
                }

                // Hoja 3: Incidencias
                if (auditoria.Incidencias.Any())
                {
                    var wsIncidencias = workbook.Worksheets.Add("Incidencias");
                    wsIncidencias.Cell(1, 1).Value = "Tipo";
                    wsIncidencias.Cell(1, 2).Value = "Descripción";
                    wsIncidencias.Cell(1, 3).Value = "Estado";
                    wsIncidencias.Cell(1, 4).Value = "Fecha Detección";

                    row = 2;
                    foreach (var incidencia in auditoria.Incidencias)
                    {
                        wsIncidencias.Cell(row, 1).Value = incidencia.TipoIncidencia;
                        wsIncidencias.Cell(row, 2).Value = incidencia.Descripcion;
                        wsIncidencias.Cell(row, 3).Value = incidencia.EstadoResolucion;
                        wsIncidencias.Cell(row, 4).Value = incidencia.FechaReporte;
                        row++;
                    }
                }

                // Ajustar columnas
                workbook.Worksheets.ToList().ForEach(ws => ws.Columns().AdjustToContents());

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte Excel de auditoría {Id}", auditoriaId);
                throw;
            }
        }

        public async Task<byte[]> GenerarReporteConsolidadoExcelAsync(ReporteConsolidadoRequestDTO request)
        {
            try
            {
                var auditorias = await _context.AuditoriasRecepcion
                    .Include(a => a.Proveedor)
                    .Include(a => a.Detalles)
                    .Include(a => a.Incidencias)
                    .Where(a => a.FechaInicio >= request.FechaDesde && a.FechaInicio <= request.FechaHasta)
                    .ToListAsync();

                if (request.ProveedoresIDs != null && request.ProveedoresIDs.Any())
                    auditorias = auditorias.Where(a => request.ProveedoresIDs.Contains(a.ProveedorID)).ToList();

                if (request.Estados != null && request.Estados.Any())
                    auditorias = auditorias.Where(a => request.Estados.Contains(a.EstadoAuditoria)).ToList();

                using var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("Reporte Consolidado");

                // Encabezados
                ws.Cell(1, 1).Value = "Número Auditoría";
                ws.Cell(1, 2).Value = "Fecha";
                ws.Cell(1, 3).Value = "Proveedor";
                ws.Cell(1, 4).Value = "OC";
                ws.Cell(1, 5).Value = "Estado";
                ws.Cell(1, 6).Value = "Total Productos";
                ws.Cell(1, 7).Value = "Total Incidencias";

                int row = 2;
                foreach (var auditoria in auditorias)
                {
                    ws.Cell(row, 1).Value = auditoria.NumeroAuditoria;
                    ws.Cell(row, 2).Value = auditoria.FechaInicio;
                    ws.Cell(row, 3).Value = auditoria.Proveedor.RazonSocial;
                    ws.Cell(row, 4).Value = auditoria.OrdenCompraID;
                    ws.Cell(row, 5).Value = auditoria.EstadoAuditoria;
                    ws.Cell(row, 6).Value = auditoria.Detalles.Count;
                    ws.Cell(row, 7).Value = auditoria.Incidencias.Count;
                    row++;
                }

                ws.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte consolidado");
                throw;
            }
        }

        public async Task<byte[]> GenerarReporteIncidenciasExcelAsync(ReporteIncidenciasRequestDTO request)
        {
            // TODO: Implementar generación de reporte de incidencias
            return await Task.FromResult(new byte[0]);
        }

        public async Task<byte[]> GenerarReporteDesempenoProveedoresAsync(ReporteProveedoresRequestDTO request)
        {
            // TODO: Implementar generación de reporte de proveedores
            return await Task.FromResult(new byte[0]);
        }

        public async Task<byte[]> GenerarReporteKPIsPDFAsync(ReporteKPIsRequestDTO request)
        {
            // TODO: Implementar generación de reporte de KPIs
            return await Task.FromResult(new byte[0]);
        }

        public async Task<byte[]> GenerarReportePersonalizadoAsync(ReportePersonalizadoRequestDTO request)
        {
            // TODO: Implementar generación de reporte personalizado
            return await Task.FromResult(new byte[0]);
        }

        public async Task<PaginatedResult<ReporteHistorialDTO>> GetHistorialReportesAsync(ReporteHistorialFiltroDTO filtro)
        {
            // TODO: Implementar historial de reportes (requiere tabla adicional)
            return await Task.FromResult(new PaginatedResult<ReporteHistorialDTO>());
        }

        public async Task<ReporteProgramadoDTO> ProgramarReporteAsync(ProgramarReporteDTO dto, int userId)
        {
            // TODO: Implementar programación de reportes (requiere tabla adicional y scheduler)
            return await Task.FromResult(new ReporteProgramadoDTO());
        }

        public async Task<ReporteProgramadoDTO> GetReporteProgramadoByIdAsync(int id)
        {
            // TODO: Implementar obtención de reporte programado
            return await Task.FromResult(new ReporteProgramadoDTO());
        }

        public async Task CancelarReporteProgramadoAsync(int id)
        {
            // TODO: Implementar cancelación de reporte programado
            await Task.CompletedTask;
        }
    }
}