import { apiService } from './api.service';
import { 
  Incidencia, 
  CrearIncidenciaData,
  ActualizarIncidenciaData,
} from '@types/incidencia.types';
import { ApiResponse, PaginatedResponse, PaginationParams } from '@types/api.types';

const getIncidencias = async (
  pagination: PaginationParams,
  filters?: any
): Promise<PaginatedResponse<Incidencia>> => {
  const response = await apiService.get<PaginatedResponse<Incidencia>>('/incidencias', {
    params: {
      page: pagination.page,
      pageSize: pagination.pageSize,
      ...filters,
    },
  });
  return response.data;
};

const getIncidenciaById = async (id: number): Promise<Incidencia> => {
  const response = await apiService.get<ApiResponse<Incidencia>>(`/incidencias/${id}`);
  return response.data.data;
};

const createIncidencia = async (data: CrearIncidenciaData): Promise<Incidencia> => {
  const response = await apiService.post<ApiResponse<Incidencia>>('/incidencias', data);
  return response.data.data;
};

const updateIncidencia = async (
  id: number, 
  data: ActualizarIncidenciaData
): Promise<Incidencia> => {
  const response = await apiService.put<ApiResponse<Incidencia>>(`/incidencias/${id}`, data);
  return response.data.data;
};

const resolverIncidencia = async (id: number, resolucion: string): Promise<Incidencia> => {
  const response = await apiService.post<ApiResponse<Incidencia>>(
    `/incidencias/${id}/resolver`,
    { resolucion }
  );
  return response.data.data;
};

const cerrarIncidencia = async (id: number): Promise<Incidencia> => {
  const response = await apiService.post<ApiResponse<Incidencia>>(`/incidencias/${id}/cerrar`);
  return response.data.data;
};

const getEstadisticas = async (): Promise<any> => {
  const response = await apiService.get<ApiResponse<any>>('/incidencias/estadisticas');
  return response.data.data;
};

export const incidenciasService = {
  getIncidencias,
  getIncidenciaById,
  createIncidencia,
  updateIncidencia,
  resolverIncidencia,
  cerrarIncidencia,
  getEstadisticas,
};