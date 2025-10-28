import { apiService } from './api.service';
import { 
  Auditoria, 
  AuditoriaFormData,
  ProductoAuditoria,
  AgregarProductoData,
  Evidencia,
} from '@types/auditoria.types';
import { ApiResponse, PaginatedResponse, PaginationParams } from '@types/api.types';

const getAuditorias = async (
  pagination: PaginationParams,
  filters?: any
): Promise<PaginatedResponse<Auditoria>> => {
  const response = await apiService.get<PaginatedResponse<Auditoria>>('/auditorias', {
    params: {
      page: pagination.page,
      pageSize: pagination.pageSize,
      ...filters,
    },
  });
  return response.data;
};

const getAuditoriaById = async (id: number): Promise<Auditoria> => {
  const response = await apiService.get<ApiResponse<Auditoria>>(`/auditorias/${id}`);
  return response.data.data;
};

const createAuditoria = async (data: AuditoriaFormData): Promise<Auditoria> => {
  const response = await apiService.post<ApiResponse<Auditoria>>('/auditorias', data);
  return response.data.data;
};

const updateAuditoria = async (id: number, data: Partial<AuditoriaFormData>): Promise<Auditoria> => {
  const response = await apiService.put<ApiResponse<Auditoria>>(`/auditorias/${id}`, data);
  return response.data.data;
};

const finalizarAuditoria = async (id: number): Promise<Auditoria> => {
  const response = await apiService.post<ApiResponse<Auditoria>>(`/auditorias/${id}/finalizar`);
  return response.data.data;
};

const cerrarAuditoria = async (id: number): Promise<Auditoria> => {
  const response = await apiService.post<ApiResponse<Auditoria>>(`/auditorias/${id}/cerrar`);
  return response.data.data;
};

const getEstadisticas = async (): Promise<any> => {
  const response = await apiService.get<ApiResponse<any>>('/auditorias/estadisticas');
  return response.data.data;
};

const getProductosAuditoria = async (auditoriaId: number): Promise<ProductoAuditoria[]> => {
  const response = await apiService.get<ApiResponse<ProductoAuditoria[]>>(
    `/auditorias/${auditoriaId}/productos`
  );
  return response.data.data;
};

const agregarProducto = async (data: AgregarProductoData): Promise<ProductoAuditoria> => {
  const response = await apiService.post<ApiResponse<ProductoAuditoria>>(
    `/auditorias/${data.auditoriaId}/productos`,
    data
  );
  return response.data.data;
};

const getEvidencias = async (auditoriaId: number): Promise<Evidencia[]> => {
  const response = await apiService.get<ApiResponse<Evidencia[]>>(
    `/auditorias/${auditoriaId}/evidencias`
  );
  return response.data.data;
};

const subirEvidencia = async (auditoriaId: number, formData: FormData): Promise<Evidencia> => {
  const response = await apiService.post<ApiResponse<Evidencia>>(
    `/auditorias/${auditoriaId}/evidencias`,
    formData,
    {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    }
  );
  return response.data.data;
};

export const auditoriasService = {
  getAuditorias,
  getAuditoriaById,
  createAuditoria,
  updateAuditoria,
  finalizarAuditoria,
  cerrarAuditoria,
  getEstadisticas,
  getProductosAuditoria,
  agregarProducto,
  getEvidencias,
  subirEvidencia,
};