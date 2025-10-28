// Exportar todos los stores
export { useAuthStore } from './authSlice';
export { useAuditoriaStore } from './auditoriaSlice';
export { useUIStore } from './uiSlice';

// Re-exportar tipos útiles
export type { Notification, NotificationType } from './uiSlice';