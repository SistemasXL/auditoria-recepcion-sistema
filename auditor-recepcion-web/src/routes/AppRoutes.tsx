import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { MainLayout } from '../layouts/MainLayout';
import { AuthLayout } from '../layouts/AuthLayout';
import { PrivateRoute } from './PrivateRoute';
import { RoleRoute } from './RoleRoute';
import { RolUsuario } from '@types/usuario.types';

// Auth Pages
import { LoginPage } from '@pages/auth/LoginPage';

// Dashboard
import { DashboardPage } from '@pages/dashboard/DashboardPage';

// Auditorías
import { ListaAuditoriasPage } from '@pages/auditorias/ListaAuditoriasPage';
import { CrearAuditoriaPage } from '@pages/auditorias/CrearAuditoriaPage';
import { DetalleAuditoriaPage } from '@pages/auditorias/DetalleAuditoriaPage';
import { EscaneoProductosPage } from '@pages/auditorias/EscaneoProductosPage';

// Productos
import { ListaProductosPage } from '@pages/productos/ListaProductosPage';

// Incidencias
import { ListaIncidenciasPage } from '@pages/incidencias/ListaIncidenciasPage';

// Reportes
import { ReportesPage } from '@pages/reportes/ReportesPage';

// Admin
import { UsuariosPage } from '@pages/admin/UsuariosPage';

export const AppRoutes: React.FC = () => {
  return (
    <Routes>
      {/* Rutas públicas - Auth */}
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/forgot-password" element={<div>Recuperar Contraseña (TODO)</div>} />
      </Route>

      {/* Rutas privadas - Main App */}
      <Route
        element={
          <PrivateRoute>
            <MainLayout />
          </PrivateRoute>
        }
      >
        {/* Dashboard */}
        <Route
          path="/dashboard"
          element={
            <RoleRoute
              allowedRoles={[
                RolUsuario.JEFE_LOGISTICA,
                RolUsuario.ADMINISTRADOR,
              ]}
            >
              <DashboardPage />
            </RoleRoute>
          }
        />

        {/* Auditorías */}
        <Route path="/auditorias">
          <Route index element={<ListaAuditoriasPage />} />
          <Route path="crear" element={<CrearAuditoriaPage />} />
          <Route path=":id" element={<DetalleAuditoriaPage />} />
        </Route>

        {/* Escaneo */}
        <Route path="/escanear" element={<EscaneoProductosPage />} />

        {/* Productos */}
        <Route path="/productos">
          <Route index element={<ListaProductosPage />} />
          <Route path=":id" element={<div>Detalle Producto (TODO)</div>} />
        </Route>

        {/* Incidencias */}
        <Route path="/incidencias">
          <Route index element={<ListaIncidenciasPage />} />
          <Route path=":id" element={<div>Detalle Incidencia (TODO)</div>} />
        </Route>

        {/* Reportes */}
        <Route
          path="/reportes"
          element={
            <RoleRoute
              allowedRoles={[
                RolUsuario.JEFE_LOGISTICA,
                RolUsuario.COMPRADOR,
                RolUsuario.ADMINISTRADOR,
              ]}
            >
              <ReportesPage />
            </RoleRoute>
          }
        />

        {/* Administración */}
        <Route path="/admin">
          <Route
            path="usuarios"
            element={
              <RoleRoute allowedRoles={[RolUsuario.ADMINISTRADOR]}>
                <UsuariosPage />
              </RoleRoute>
            }
          />
          <Route
            path="configuracion"
            element={
              <RoleRoute allowedRoles={[RolUsuario.ADMINISTRADOR]}>
                <div>Configuración (TODO)</div>
              </RoleRoute>
            }
          />
        </Route>

        {/* Perfil */}
        <Route path="/perfil" element={<div>Mi Perfil (TODO)</div>} />

        {/* Redirect root to dashboard or auditorías */}
        <Route path="/" element={<Navigate to="/auditorias" replace />} />
      </Route>

      {/* 404 - Not Found */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};