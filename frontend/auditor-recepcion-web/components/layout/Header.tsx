import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  AppBar,
  Toolbar,
  IconButton,
  Typography,
  Box,
  Avatar,
  Menu,
  MenuItem,
  Badge,
  Divider,
  ListItemIcon,
  ListItemText,
} from '@mui/material';
import {
  Menu as MenuIcon,
  Notifications as NotificationsIcon,
  AccountCircle,
  Logout,
  Settings,
  QrCodeScanner,
} from '@mui/icons-material';
import { useUIStore } from '@store/uiSlice';
import { useAuth } from '@hooks/useAuth';
import { getInitials, getAvatarColor } from '@utils/formatters';

export const Header: React.FC = () => {
  const navigate = useNavigate();
  const { toggleSidebar } = useUIStore();
  const { usuario, logout } = useAuth();
  
  const [anchorElUser, setAnchorElUser] = useState<null | HTMLElement>(null);
  const [anchorElNotif, setAnchorElNotif] = useState<null | HTMLElement>(null);

  const handleOpenUserMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorElUser(event.currentTarget);
  };

  const handleCloseUserMenu = () => {
    setAnchorElUser(null);
  };

  const handleOpenNotifications = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorElNotif(event.currentTarget);
  };

  const handleCloseNotifications = () => {
    setAnchorElNotif(null);
  };

  const handleLogout = async () => {
    handleCloseUserMenu();
    await logout();
    navigate('/login');
  };

  const handleProfile = () => {
    handleCloseUserMenu();
    navigate('/perfil');
  };

  const handleScanner = () => {
    navigate('/escanear');
  };

  return (
    <AppBar
      position="fixed"
      sx={{
        zIndex: (theme) => theme.zIndex.drawer + 1,
        backgroundColor: 'white',
        color: 'text.primary',
        boxShadow: 1,
      }}
    >
      <Toolbar>
        <IconButton
          color="inherit"
          aria-label="open drawer"
          edge="start"
          onClick={toggleSidebar}
          sx={{ mr: 2 }}
        >
          <MenuIcon />
        </IconButton>

        <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
          Sistema de Auditoría de Recepción
        </Typography>

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          {/* Botón de escáner - Solo visible en desktop */}
          <IconButton
            color="primary"
            onClick={handleScanner}
            sx={{ display: { xs: 'none', md: 'flex' } }}
          >
            <QrCodeScanner />
          </IconButton>

          {/* Notificaciones */}
          <IconButton color="inherit" onClick={handleOpenNotifications}>
            <Badge badgeContent={3} color="error">
              <NotificationsIcon />
            </Badge>
          </IconButton>

          {/* Avatar y menú de usuario */}
          <IconButton onClick={handleOpenUserMenu} sx={{ p: 0, ml: 1 }}>
            <Avatar
              sx={{
                bgcolor: usuario ? getAvatarColor(usuario.nombreCompleto) : 'primary.main',
                width: 40,
                height: 40,
              }}
            >
              {usuario ? getInitials(usuario.nombreCompleto) : 'U'}
            </Avatar>
          </IconButton>
        </Box>

        {/* Menú de notificaciones */}
        <Menu
          anchorEl={anchorElNotif}
          open={Boolean(anchorElNotif)}
          onClose={handleCloseNotifications}
          anchorOrigin={{
            vertical: 'bottom',
            horizontal: 'right',
          }}
          transformOrigin={{
            vertical: 'top',
            horizontal: 'right',
          }}
        >
          <MenuItem>
            <Typography variant="body2">Nueva incidencia reportada</Typography>
          </MenuItem>
          <MenuItem>
            <Typography variant="body2">Auditoría pendiente de revisión</Typography>
          </MenuItem>
          <MenuItem>
            <Typography variant="body2">Desviación detectada en OC-2025-042</Typography>
          </MenuItem>
          <Divider />
          <MenuItem onClick={handleCloseNotifications}>
            <Typography variant="body2" color="primary">
              Ver todas las notificaciones
            </Typography>
          </MenuItem>
        </Menu>

        {/* Menú de usuario */}
        <Menu
          anchorEl={anchorElUser}
          open={Boolean(anchorElUser)}
          onClose={handleCloseUserMenu}
          anchorOrigin={{
            vertical: 'bottom',
            horizontal: 'right',
          }}
          transformOrigin={{
            vertical: 'top',
            horizontal: 'right',
          }}
        >
          <Box sx={{ px: 2, py: 1 }}>
            <Typography variant="subtitle1">{usuario?.nombreCompleto}</Typography>
            <Typography variant="body2" color="textSecondary">
              {usuario?.rol}
            </Typography>
          </Box>
          <Divider />
          <MenuItem onClick={handleProfile}>
            <ListItemIcon>
              <AccountCircle fontSize="small" />
            </ListItemIcon>
            <ListItemText>Mi Perfil</ListItemText>
          </MenuItem>
          <MenuItem onClick={handleCloseUserMenu}>
            <ListItemIcon>
              <Settings fontSize="small" />
            </ListItemIcon>
            <ListItemText>Configuración</ListItemText>
          </MenuItem>
          <Divider />
          <MenuItem onClick={handleLogout}>
            <ListItemIcon>
              <Logout fontSize="small" />
            </ListItemIcon>
            <ListItemText>Cerrar Sesión</ListItemText>
          </MenuItem>
        </Menu>
      </Toolbar>
    </AppBar>
  );
};