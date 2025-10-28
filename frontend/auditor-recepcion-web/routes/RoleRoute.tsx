import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '@hooks/useAuth';
import { RolUsuario } from '@types/usuario.types';
import { Box, Typography, Button } from '@mui/material';
import { Lock as LockIcon } from '@mui/icons-material';

interface RoleRouteProps {
  children: React.ReactNode;
  allowedRoles: RolUsuario[];
}

export const RoleRoute: React.FC<RoleRouteProps> = ({ children, allowedRoles }) => {
  const { hasRole, usuario } = useAuth();

  if (!hasRole(...allowedRoles)) {
    return (
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: '60vh',
          textAlign: 'center',
          gap: 3,
        }}
      >
        <LockIcon sx={{ fontSize: 80, color: 'text.secondary' }} />
        <Typography variant="h4" gutterBottom>
          Acceso Denegado
        </Typography>
        <Typography variant="body1" color="textSecondary" sx={{ maxWidth: 500 }}>
          No tienes permisos para acceder a esta sección. Tu rol actual es: <strong>{usuario?.rol}</strong>
        </Typography>
        <Button variant="contained" href="/dashboard">
          Volver al Dashboard
        </Button>
      </Box>
    );
  }

  return <>{children}</>;
};