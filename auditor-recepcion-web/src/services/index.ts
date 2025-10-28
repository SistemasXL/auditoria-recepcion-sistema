// Exportar todos los servicios desde un único punto
export { apiService } from './api.service';
export { authService } from './auth.service';
export { auditoriasService } from './auditorias.service';
export { productosService } from './productos.service';
export { incidenciasService } from './incidencias.service';
export { reportesService } from './reportes.service';

// Re-exportar tipos de reportes
export type { 
  ReporteAuditoriasParams,
  ReporteIncidenciasParams,
  ReporteProveedoresParams
} from './reportes.service';