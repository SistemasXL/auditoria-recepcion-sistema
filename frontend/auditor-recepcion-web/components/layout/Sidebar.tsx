import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Divider,
  Box,
  IconButton,
  Typography,
  Collapse,
} from '@mui/material';
import {
  Dashboard as DashboardIcon,
  Assignment as AssignmentIcon,
  Inventory as InventoryIcon,
  Warning as WarningIcon,
  Assessment as AssessmentIcon,
  People as PeopleIcon,
  Settings as SettingsIcon,
  ChevronLeft as ChevronLeftIcon,
  ExpandLess,
  ExpandMore,
} from '@mui/icons-material';
import { useUIStore } from '@store/uiSlice';
import { useAuth } from '@hooks/useAuth';
import { RolUsuario } from '@types/usuario.types';

const DRAWER_WIDTH = 260;

interface MenuItem {
  id: string;
  label: string;
  icon: React.ReactNode;
  path?: string;
  roles?: RolUsuario[];
  permission?: string;
  children?: MenuItem[];
}

const menuItems: MenuItem[] = [
  {
    id: 'dashboard',
    label: 'Dashboard',
    icon: <DashboardIcon />,
    path: '/dashboard',
    roles: [RolUsuario.JEFE_LOGISTICA, RolUsuario.ADMINISTRADOR],
  },
  {
    id: 'auditorias',
    label: 'Auditorías',
    icon: <AssignmentIcon />,
    path: '/auditorias',
  },
  {
    id: 'productos',
    label: 'Productos',
    icon: <InventoryIcon />,
    path: '/productos',
  },
  {
    id: 'incidencias',
    label: 'Incidencias',
    icon: <WarningIcon />,
    path: '/incidencias',
  },
  {
    id: 'reportes',
    label: 'Reportes',
    icon: <AssessmentIcon />,
    path: '/reportes',
    roles: [RolUsuario.JEFE_LOGISTICA, RolUsuario.COMPRADOR, RolUsuario.ADMINISTRADOR],
  },
  {
    id: 'admin',
    label: 'Administración',
    icon: <SettingsIcon />,
    roles: [RolUsuario.ADMINISTRADOR],
    children: [
      {
        id: 'usuarios',
        label: 'Usuarios',
        icon: <PeopleIcon />,
        path: '/admin/usuarios',
      },
      {
        id: 'configuracion',
        label: 'Configuración',
        icon: <SettingsIcon />,
        path: '/admin/configuracion',
      },
    ],
  },
];

export const Sidebar: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { sidebarOpen, sidebarCollapsed, setSidebarOpen, toggleSidebarCollapse } = useUIStore();
  const { hasRole } = useAuth();
  const [expandedItems, setExpandedItems] = React.useState<string[]>([]);

  const handleToggle = (itemId: string) => {
    setExpandedItems((prev) =>
      prev.includes(itemId)
        ? prev.filter((id) => id !== itemId)
        : [...prev, itemId]
    );
  };

  const handleNavigation = (path: string) => {
    navigate(path);
    // En móvil, cerrar el sidebar después de navegar
    if (window.innerWidth < 900) {
      setSidebarOpen(false);
    }
  };

  const isActiveRoute = (path?: string) => {
    if (!path) return false;
    return location.pathname === path || location.pathname.startsWith(path + '/');
  };

  const canAccessMenuItem = (item: MenuItem): boolean => {
    if (!item.roles) return true;
    return hasRole(...item.roles);
  };

  const renderMenuItem = (item: MenuItem, depth: number = 0) => {
    if (!canAccessMenuItem(item)) return null;

    const hasChildren = item.children && item.children.length > 0;
    const isExpanded = expandedItems.includes(item.id);
    const isActive = isActiveRoute(item.path);

    if (hasChildren) {
      return (
        <React.Fragment key={item.id}>
          <ListItem disablePadding>
            <ListItemButton
              onClick={() => handleToggle(item.id)}
              sx={{ pl: 2 + depth * 2 }}
            >
              <ListItemIcon sx={{ minWidth: 40 }}>{item.icon}</ListItemIcon>
              {!sidebarCollapsed && (
                <>
                  <ListItemText primary={item.label} />
                  {isExpanded ? <ExpandLess /> : <ExpandMore />}
                </>
              )}
            </ListItemButton>
          </ListItem>
          <Collapse in={isExpanded && !sidebarCollapsed} timeout="auto" unmountOnExit>
            <List component="div" disablePadding>
              {item.children?.map((child) => renderMenuItem(child, depth + 1))}
            </List>
          </Collapse>
        </React.Fragment>
      );
    }

    return (
      <ListItem key={item.id} disablePadding>
        <ListItemButton
          onClick={() => item.path && handleNavigation(item.path)}
          selected={isActive}
          sx={{
            pl: 2 + depth * 2,
            '&.Mui-selected': {
              backgroundColor: 'primary.light',
              color: 'primary.main',
              '&:hover': {
                backgroundColor: 'primary.light',
              },
            },
          }}
        >
          <ListItemIcon sx={{ minWidth: 40, color: isActive ? 'primary.main' : 'inherit' }}>
            {item.icon}
          </ListItemIcon>
          {!sidebarCollapsed && <ListItemText primary={item.label} />}
        </ListItemButton>
      </ListItem>
    );
  };

  return (
    <Drawer
      variant="permanent"
      open={sidebarOpen}
      sx={{
        width: sidebarCollapsed ? 70 : DRAWER_WIDTH,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: sidebarCollapsed ? 70 : DRAWER_WIDTH,
          boxSizing: 'border-box',
          transition: 'width 0.3s',
          overflowX: 'hidden',
        },
      }}
    >
      <Box
        sx={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          p: 2,
          minHeight: 64,
        }}
      >
        {!sidebarCollapsed && (
          <Typography variant="h6" noWrap>
            Auditoría
          </Typography>
        )}
        <IconButton onClick={toggleSidebarCollapse}>
          <ChevronLeftIcon />
        </IconButton>
      </Box>

      <Divider />

      <List sx={{ flexGrow: 1 }}>
        {menuItems.map((item) => renderMenuItem(item))}
      </List>

      <Divider />

      <Box sx={{ p: 2 }}>
        <Typography variant="caption" color="textSecondary" align="center" display="block">
          v1.0.0
        </Typography>
      </Box>
    </Drawer>
  );
};