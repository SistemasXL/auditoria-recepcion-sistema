export const API_CONFIG = {
  BASE_URL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
  TIMEOUT: 30000,
  RETRY_ATTEMPTS: 3,
  RETRY_DELAY: 1000,
};

export const ENDPOINTS = {
  // Autenticación
  AUTH: {
    LOGIN: '/auth/login',
    LOGOUT: '/auth/logout',
    REFRESH: '/auth/refresh',
    ME: '/auth/me',
    CHANGE_PASSWORD: '/auth/change-password',
  },
  
  // Auditorías
  AUDITORIAS: {
    BASE: '/auditorias',
    BY_ID: (id: number) => `/auditorias/${id}`,
    PRODUCTOS: (id: number) => `/auditorias/${id}/productos`,
    EVIDENCIAS: (id: number) => `/auditorias/${id}/evidencias`,
    FINALIZAR: (id: number) => `/auditorias/${id}/finalizar`,
    CERRAR: (id: number) => `/auditorias/${id}/cerrar`,
    ESTADISTICAS: '/auditorias/estadisticas',
  },
  
  // Productos
  PRODUCTOS: {
    BASE: '/productos',
    BY_ID: (id: number) => `/productos/${id}`,
    BY_SKU: (sku: string) => `/productos/sku/${sku}`,
    BY_BARCODE: (barcode: string) => `/productos/barcode/${barcode}`,
    SEARCH: '/productos/search',
  },
  
  // Incidencias
  INCIDENCIAS: {
    BASE: '/incidencias',
    BY_ID: (id: number) => `/incidencias/${id}`,
    COMENTARIOS: (id: number) => `/incidencias/${id}/comentarios`,
    ESTADISTICAS: '/incidencias/estadisticas',
  },
  
  // Proveedores
  PROVEEDORES: {
    BASE: '/proveedores',
    BY_ID: (id: number) => `/proveedores/${id}`,
    SEARCH: '/proveedores/search',
  },
  
  // Usuarios
  USUARIOS: {
    BASE: '/usuarios',
    BY_ID: (id: number) => `/usuarios/${id}`,
    ACCIONES: '/usuarios/acciones',
  },
  
  // Reportes
  REPORTES: {
    AUDITORIAS: '/reportes/auditorias',
    INCIDENCIAS: '/reportes/incidencias',
    PROVEEDORES: '/reportes/proveedores',
    EXPORT_PDF: '/reportes/export/pdf',
    EXPORT_EXCEL: '/reportes/export/excel',
  },
  
  // Notificaciones
  NOTIFICACIONES: {
    BASE: '/notificaciones',
    MARK_READ: (id: number) => `/notificaciones/${id}/read`,
    MARK_ALL_READ: '/notificaciones/read-all',
  },
};

export const FILE_CONFIG = {
  MAX_SIZE: parseInt(import.meta.env.VITE_MAX_FILE_SIZE || '10485760'), // 10MB
  ALLOWED_TYPES: (import.meta.env.VITE_ALLOWED_FILE_TYPES || 'image/jpeg,image/png,image/jpg,video/mp4,application/pdf').split(','),
  ALLOWED_EXTENSIONS: ['jpg', 'jpeg', 'png', 'mp4', 'pdf'],
};