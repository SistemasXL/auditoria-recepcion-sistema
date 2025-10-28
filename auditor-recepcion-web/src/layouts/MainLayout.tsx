import React from 'react';
import { Outlet } from 'react-router-dom';
import { Box, Container, Toolbar } from '@mui/material';
import { Header } from '@components/layout/Header';
import { Sidebar } from '@components/layout/Sidebar';
import { useUIStore } from '@store/uiSlice';

export const MainLayout: React.FC = () => {
  const { sidebarCollapsed } = useUIStore();

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <Header />
      <Sidebar />
      
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          backgroundColor: (theme) => theme.palette.grey[50],
          minHeight: '100vh',
          ml: sidebarCollapsed ? '70px' : '260px',
          transition: 'margin-left 0.3s',
        }}
      >
        {/* Espaciador para el AppBar fijo */}
        <Toolbar />
        
        <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
          <Outlet />
        </Container>
      </Box>
    </Box>
  );
};