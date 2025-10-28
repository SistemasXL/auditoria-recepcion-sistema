import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '@store/authSlice';
import { RolUsuario } from '@types/usuario.types';
import { PERMISOS_POR_ROL } from '@config/constants';

export const useAuth = () => {
  const navigate = useNavigate();
  const {
    usuario,
    isAuthenticated,
    isLoading,
    error,
    login,
    logout,
    loadUser,
    clearError,
    checkAuth,
  } = useAuthStore();

  // Verificar autenticación al montar
  useEffect(() => {
    if (!isAuthenticated) {
      checkAuth();
    }
  }, []);

  /**
   * Verificar si el usuario tiene un permiso específico
   */
  const hasPermission = (permission: string): boolean => {
    if (!usuario || !usuario.rol) return false;

    const permisos = PERMISOS_POR_ROL[usuario.rol as RolUsuario] || [];
    
    // Si tiene permiso de administrador completo
    if (permisos.includes('*')) return true;

    return permisos.includes(permission);
  };

  /**
   * Verificar si el usuario tiene alguno de los roles especificados
   */
  const hasRole = (...roles: RolUsuario[]): boolean => {
    if (!usuario || !usuario.rol) return false;
    return roles.includes(usuario.rol as RolUsuario);
  };

  /**
   * Redireccionar si no está autenticado
   */
  const requireAuth = (redirectTo: string = '/login') => {
    if (!isAuthenticated) {
      navigate(redirectTo);
      return false;
    }
    return true;
  };

  /**
   * Redireccionar si no tiene el rol requerido
   */
  const requireRole = (roles: RolUsuario[], redirectTo: string = '/') => {
    if (!hasRole(...roles)) {
      navigate(redirectTo);
      return false;
    }
    return true;
  };

  return {
    // Estado
    usuario,
    isAuthenticated,
    isLoading,
    error,

    // Acciones
    login,
    logout,
    loadUser,
    clearError,

    // Utilidades
    hasPermission,
    hasRole,
    requireAuth,
    requireRole,
  };
};