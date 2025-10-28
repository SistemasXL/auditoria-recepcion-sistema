import { apiService } from './api.service';

interface ReporteParams {
  dateFrom?: string;
  dateTo?: string;
  proveedorId?: number;
  estado?: string;
}

const exportarPDF = async (tipo: string, params?: ReporteParams): Promise<void> => {
  const response = await apiService.get(`/reportes/${tipo}/pdf`, {
    params,
    responseType: 'blob',
  });

  const url = window.URL.createObjectURL(new Blob([response.data]));
  const link = document.createElement('a');
  link.href = url;
  link.setAttribute('download', `reporte-${tipo}-${new Date().getTime()}.pdf`);
  document.body.appendChild(link);
  link.click();
  link.remove();
};

const exportarExcel = async (tipo: string, params?: ReporteParams): Promise<void> => {
  const response = await apiService.get(`/reportes/${tipo}/excel`, {
    params,
    responseType: 'blob',
  });

  const url = window.URL.createObjectURL(new Blob([response.data]));
  const link = document.createElement('a');
  link.href = url;
  link.setAttribute('download', `reporte-${tipo}-${new Date().getTime()}.xlsx`);
  document.body.appendChild(link);
  link.click();
  link.remove();
};

export const reportesService = {
  exportarPDF,
  exportarExcel,
};