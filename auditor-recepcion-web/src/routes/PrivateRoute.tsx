import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '@hooks/useAuth';
import { Loader } from '@components/common/Loader';

interface PrivateRouteProps {
  children: React.ReactNode;
}

export const PrivateRoute: React.FC<PrivateRouteProps> = ({ children }) => {
  // 🔓 COMENTAR TEMPORALMENTE PARA DESARROLLO
  // const { isAuthenticated, isLoading } = useAuth();
  // const location = useLocation();

  // if (isLoading) {
  //   return <Loader fullScreen message="Verificando autenticación..." />;
  // }

  // if (!isAuthenticated) {
  //   return <Navigate to="/login" state={{ from: location }} replace />;
  // }

  // ✅ PERMITIR ACCESO SIN AUTENTICACIÓN (SOLO PARA DESARROLLO)
  return <>{children}</>;
};