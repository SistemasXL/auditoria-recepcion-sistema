import { apiService } from './api.service';
import { ENDPOINTS } from '@config/api.config';
import { 
  Incidencia, 
  CrearIncidenciaDto, 
  ActualizarIncidenciaDto,
  ComentarioIncidencia,
  EstadisticasIncidencias,
  EstadoIncidencia
} from '@types/incidencia.types';
import { PaginatedResponse, PaginationParams, FilterParams } from '@types/api.types';

class IncidenciasService {
  /**
   * Obtener lista de incidencias con paginación y filtros
   */
  async getIncidencias(
    pagination: PaginationParams,
    filters?: FilterParams
  ): Promise<PaginatedResponse<Incidencia>> {
    const params = {
      ...pagination,
      ...filters,
    };

    const response = await apiService.get<PaginatedResponse<Incidencia>>(
      ENDPOINTS.INCIDENCIAS.BASE,
      { params }
    );

    return response.data;
  }

  /**
   * Obtener detalle de una incidencia
   */
  async getIncidenciaById(id: number): Promise<Incidencia> {
    const response = await apiService.get<Incidencia>(
      ENDPOINTS.INCIDENCIAS.BY_ID(id)
    );
    return response.data;
  }

  /**
   * Crear nueva incidencia
   */
  async createIncidencia(data: CrearIncidenciaDto): Promise<Incidencia> {
    const response = await apiService.post<Incidencia>(
      ENDPOINTS.INCIDENCIAS.BASE,
      data
    );
    return response.data;
  }

  /**
   * Actualizar incidencia
   */
  async updateIncidencia(id: number, data: ActualizarIncidenciaDto): Promise<Incidencia> {
    const response = await apiService.put<Incidencia>(
      ENDPOINTS.INCIDENCIAS.BY_ID(id),
      data
    );
    return response.data;
  }

  /**
   * Cambiar estado de incidencia
   */
  async cambiarEstado(id: number, estado: EstadoIncidencia): Promise<Incidencia> {
    const response = await apiService.patch<Incidencia>(
      ENDPOINTS.INCIDENCIAS.BY_ID(id),
      { estado }
    );
    return response.data;
  }

  /**
   * Agregar comentario a incidencia
   */
  async agregarComentario(
    incidenciaId: number,
    comentario: string
  ): Promise<ComentarioIncidencia> {
    const response = await apiService.post<ComentarioIncidencia>(
      ENDPOINTS.INCIDENCIAS.COMENTARIOS(incidenciaId),
      { comentario }
    );
    return response.data;
  }

  /**
   * Obtener comentarios de una incidencia
   */
  async getComentarios(incidenciaId: number): Promise<ComentarioIncidencia[]> {
    const response = await apiService.get<ComentarioIncidencia[]>(
      ENDPOINTS.INCIDENCIAS.COMENTARIOS(incidenciaId)
    );
    return response.data;
  }

  /**
   * Obtener incidencias por auditoría
   */
  async getIncidenciasByAuditoria(auditoriaId: number): Promise<Incidencia[]> {
    const response = await apiService.get<PaginatedResponse<Incidencia>>(
      ENDPOINTS.INCIDENCIAS.BASE,
      { params: { auditoriaId } }
    );
    return response.data.items;
  }

  /**
   * Obtener incidencias abiertas
   */
  async getIncidenciasAbiertas(): Promise<Incidencia[]> {
    const response = await apiService.get<PaginatedResponse<Incidencia>>(
      ENDPOINTS.INCIDENCIAS.BASE,
      { params: { estado: EstadoIncidencia.ABIERTA } }
    );
    return response.data.items;
  }

  /**
   * Obtener estadísticas de incidencias
   */
  async getEstadisticas(dateFrom?: string, dateTo?: string): Promise<EstadisticasIncidencias> {
    const params: any = {};
    if (dateFrom) params.dateFrom = dateFrom;
    if (dateTo) params.dateTo = dateTo;

    const response = await apiService.get<EstadisticasIncidencias>(
      ENDPOINTS.INCIDENCIAS.ESTADISTICAS,
      { params }
    );
    return response.data;
  }

  /**
   * Asignar responsable a incidencia
   */
  async asignarResponsable(incidenciaId: number, responsableId: number): Promise<Incidencia> {
    const response = await apiService.patch<Incidencia>(
      ENDPOINTS.INCIDENCIAS.BY_ID(incidenciaId),
      { responsableId }
    );
    return response.data;
  }
}

export const incidenciasService = new IncidenciasService();