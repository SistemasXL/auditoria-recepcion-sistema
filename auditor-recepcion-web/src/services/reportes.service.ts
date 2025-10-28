import { apiService } from './api.service';
import { ENDPOINTS } from '@config/api.config';

export interface ReporteAuditoriasParams {
  dateFrom?: string;
  dateTo?: string;
  proveedorId?: number;
  estado?: string;
  usuarioId?: number;
}

export interface ReporteIncidenciasParams {
  dateFrom?: string;
  dateTo?: string;
  tipo?: string;
  severidad?: string;
  estado?: string;
  proveedorId?: number;
}

export interface ReporteProveedoresParams {
  dateFrom?: string;
  dateTo?: string;
  proveedorId?: number;
}

class ReportesService {
  /**
   * Obtener datos de reporte de auditorías
   */
  async getReporteAuditorias(params: ReporteAuditoriasParams): Promise<any> {
    const response = await apiService.get(
      ENDPOINTS.REPORTES.AUDITORIAS,
      { params }
    );
    return response.data;
  }

  /**
   * Obtener datos de reporte de incidencias
   */
  async getReporteIncidencias(params: ReporteIncidenciasParams): Promise<any> {
    const response = await apiService.get(
      ENDPOINTS.REPORTES.INCIDENCIAS,
      { params }
    );
    return response.data;
  }

  /**
   * Obtener datos de reporte de proveedores
   */
  async getReporteProveedores(params: ReporteProveedoresParams): Promise<any> {
    const response = await apiService.get(
      ENDPOINTS.REPORTES.PROVEEDORES,
      { params }
    );
    return response.data;
  }

  /**
   * Exportar reporte a PDF
   */
  async exportarPDF(tipo: 'auditorias' | 'incidencias' | 'proveedores', params: any): Promise<void> {
    const queryParams = new URLSearchParams({
      tipo,
      ...params,
    }).toString();

    const filename = `reporte_${tipo}_${new Date().getTime()}.pdf`;
    
    await apiService.downloadFile(
      `${ENDPOINTS.REPORTES.EXPORT_PDF}?${queryParams}`,
      filename
    );
  }

  /**
   * Exportar reporte a Excel
   */
  async exportarExcel(tipo: 'auditorias' | 'incidencias' | 'proveedores', params: any): Promise<void> {
    const queryParams = new URLSearchParams({
      tipo,
      ...params,
    }).toString();

    const filename = `reporte_${tipo}_${new Date().getTime()}.xlsx`;
    
    await apiService.downloadFile(
      `${ENDPOINTS.REPORTES.EXPORT_EXCEL}?${queryParams}`,
      filename
    );
  }

  /**
   * Generar reporte de auditoría específica en PDF
   */
  async generarReporteAuditoriaPDF(auditoriaId: number): Promise<void> {
    const filename = `auditoria_${auditoriaId}_${new Date().getTime()}.pdf`;
    
    await apiService.downloadFile(
      `${ENDPOINTS.REPORTES.EXPORT_PDF}?tipo=auditoria&id=${auditoriaId}`,
      filename
    );
  }
}

export const reportesService = new ReportesService();