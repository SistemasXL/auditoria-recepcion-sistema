import { RolUsuario } from '@types/usuario.types';
import { EstadoAuditoria, TipoIncidencia, SeveridadIncidencia, EstadoIncidencia } from '@types/index';

export const APP_NAME = import.meta.env.VITE_APP_NAME || 'Sistema de Auditoría';
export const APP_VERSION = import.meta.env.VITE_APP_VERSION || '1.0.0';

// Configuración de paginación
export const PAGINATION = {
  DEFAULT_PAGE: 1,
  DEFAULT_PAGE_SIZE: 10,
  PAGE_SIZE_OPTIONS: [5, 10, 25, 50, 100],
};

// Configuración de localStorage
export const STORAGE_KEYS = {
  AUTH_TOKEN: 'auth_token',
  REFRESH_TOKEN: 'refresh_token',
  USER_DATA: 'user_data',
  THEME: 'theme_preference',
  LANGUAGE: 'language',
};

// Roles y permisos
export const ROLES = {
  OPERADOR: RolUsuario.OPERADOR,
  JEFE_LOGISTICA: RolUsuario.JEFE_LOGISTICA,
  COMPRADOR: RolUsuario.COMPRADOR,
  ADMINISTRADOR: RolUsuario.ADMINISTRADOR,
};

export const PERMISOS_POR_ROL: Record<RolUsuario, string[]> = {
  [RolUsuario.OPERADOR]: [
    'auditorias.crear',
    'auditorias.ver_propias',
    'auditorias.editar_propias',
    'productos.escanear',
    'evidencias.subir',
    'incidencias.crear',
  ],
  [RolUsuario.JEFE_LOGISTICA]: [
    'auditorias.ver_todas',
    'auditorias.aprobar',
    'auditorias.cerrar',
    'incidencias.ver_todas',
    'incidencias.gestionar',
    'reportes.generar',
    'dashboard.ver',
  ],
  [RolUsuario.COMPRADOR]: [
    'auditorias.ver_todas',
    'incidencias.ver_todas',
    'incidencias.comentar',
    'alertas.recibir',
    'reportes.proveedores',
  ],
  [RolUsuario.ADMINISTRADOR]: [
    'usuarios.gestionar',
    'roles.gestionar',
    'sistema.configurar',
    'auditoria.ver_logs',
    '*', // Acceso total
  ],
};

// Estados de auditoría con colores
export const ESTADOS_AUDITORIA_CONFIG = {
  [EstadoAuditoria.BORRADOR]: {
    color: '#9CA3AF',
    bgColor: '#F3F4F6',
    label: 'Borrador',
  },
  [EstadoAuditoria.EN_PROCESO]: {
    color: '#3B82F6',
    bgColor: '#DBEAFE',
    label: 'En Proceso',
  },
  [EstadoAuditoria.FINALIZADA]: {
    color: '#10B981',
    bgColor: '#D1FAE5',
    label: 'Finalizada',
  },
  [EstadoAuditoria.CERRADA]: {
    color: '#6B7280',
    bgColor: '#E5E7EB',
    label: 'Cerrada',
  },
  [EstadoAuditoria.CANCELADA]: {
    color: '#EF4444',
    bgColor: '#FEE2E2',
    label: 'Cancelada',
  },
};

// Tipos de incidencia con iconos
export const TIPOS_INCIDENCIA_CONFIG = {
  [TipoIncidencia.FALTANTE]: {
    icon: '📉',
    color: '#EF4444',
    label: 'Faltante',
  },
  [TipoIncidencia.SOBRANTE]: {
    icon: '📈',
    color: '#F59E0B',
    label: 'Sobrante',
  },
  [TipoIncidencia.PRODUCTO_DANADO]: {
    icon: '💔',
    color: '#DC2626',
    label: 'Producto Dañado',
  },
  [TipoIncidencia.PRODUCTO_INCORRECTO]: {
    icon: '❌',
    color: '#EF4444',
    label: 'Producto Incorrecto',
  },
  [TipoIncidencia.CALIDAD_DEFICIENTE]: {
    icon: '⚠️',
    color: '#F59E0B',
    label: 'Calidad Deficiente',
  },
  [TipoIncidencia.EMPAQUE_DANADO]: {
    icon: '📦',
    color: '#F59E0B',
    label: 'Empaque Dañado',
  },
  [TipoIncidencia.DOCUMENTACION_INCORRECTA]: {
    icon: '📄',
    color: '#F59E0B',
    label: 'Documentación Incorrecta',
  },
  [TipoIncidencia.OTRO]: {
    icon: '🔍',
    color: '#6B7280',
    label: 'Otro',
  },
};

// Severidad de incidencias
export const SEVERIDAD_INCIDENCIA_CONFIG = {
  [SeveridadIncidencia.BAJA]: {
    color: '#10B981',
    bgColor: '#D1FAE5',
    label: 'Baja',
    nivel: 1,
  },
  [SeveridadIncidencia.MEDIA]: {
    color: '#F59E0B',
    bgColor: '#FEF3C7',
    label: 'Media',
    nivel: 2,
  },
  [SeveridadIncidencia.ALTA]: {
    color: '#EF4444',
    bgColor: '#FEE2E2',
    label: 'Alta',
    nivel: 3,
  },
  [SeveridadIncidencia.CRITICA]: {
    color: '#DC2626',
    bgColor: '#FEE2E2',
    label: 'Crítica',
    nivel: 4,
  },
};

// Estados de incidencia
export const ESTADOS_INCIDENCIA_CONFIG = {
  [EstadoIncidencia.ABIERTA]: {
    color: '#EF4444',
    bgColor: '#FEE2E2',
    label: 'Abierta',
  },
  [EstadoIncidencia.EN_REVISION]: {
    color: '#F59E0B',
    bgColor: '#FEF3C7',
    label: 'En Revisión',
  },
  [EstadoIncidencia.RESUELTA]: {
    color: '#10B981',
    bgColor: '#D1FAE5',
    label: 'Resuelta',
  },
  [EstadoIncidencia.CERRADA]: {
    color: '#6B7280',
    bgColor: '#E5E7EB',
    label: 'Cerrada',
  },
  [EstadoIncidencia.RECHAZADA]: {
    color: '#9CA3AF',
    bgColor: '#F3F4F6',
    label: 'Rechazada',
  },
};

// Formatos de fecha
export const DATE_FORMATS = {
  DISPLAY: 'dd/MM/yyyy',
  DISPLAY_WITH_TIME: 'dd/MM/yyyy HH:mm',
  API: 'yyyy-MM-dd',
  API_WITH_TIME: "yyyy-MM-dd'T'HH:mm:ss",
  TIME_ONLY: 'HH:mm',
};

// Mensajes del sistema
export const MESSAGES = {
  SUCCESS: {
    CREATED: 'Registro creado exitosamente',
    UPDATED: 'Registro actualizado exitosamente',
    DELETED: 'Registro eliminado exitosamente',
    SAVED: 'Cambios guardados exitosamente',
  },
  ERROR: {
    GENERIC: 'Ocurrió un error. Por favor intente nuevamente',
    NETWORK: 'Error de conexión. Verifique su conexión a internet',
    UNAUTHORIZED: 'No tiene permisos para realizar esta acción',
    NOT_FOUND: 'Registro no encontrado',
    VALIDATION: 'Por favor complete todos los campos requeridos',
  },
  CONFIRM: {
    DELETE: '¿Está seguro que desea eliminar este registro?',
    CANCEL: '¿Está seguro que desea cancelar? Los cambios no guardados se perderán',
    FINALIZE: '¿Está seguro que desea finalizar esta auditoría?',
    CLOSE: '¿Está seguro que desea cerrar este registro?',
  },
};