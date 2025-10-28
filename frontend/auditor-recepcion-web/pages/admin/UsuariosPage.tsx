import React, { useState } from 'react';
import {
  Box,
  Typography,
  Chip,
} from '@mui/material';
import {
  Add as AddIcon,
} from '@mui/icons-material';
import { Table, Column } from '@components/common/Table';
import { Button } from '@components/common/Button';
import { Badge } from '@components/common/Badge';
import { Usuario } from '@types/usuario.types';
import { formatDate } from '@utils/formatters';

export const UsuariosPage: React.FC = () => {
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const [loading, setLoading] = useState(false);
  const [totalItems, setTotalItems] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const handlePageChange = (newPage: number) => {
    setCurrentPage(newPage + 1);
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize);
    setCurrentPage(1);
  };

  const handleCrearUsuario = () => {
    // TODO: Abrir modal de crear usuario
    console.log('Crear usuario');
  };

  const columns: Column<Usuario>[] = [
    {
      id: 'nombreCompleto',
      label: 'Nombre',
      render: (row) => (
        <Box>
          <Typography variant="body2" fontWeight="medium">
            {row.nombreCompleto}
          </Typography>
          <Typography variant="caption" color="textSecondary">
            {row.email}
          </Typography>
        </Box>
      ),
    },
    {
      id: 'username',
      label: 'Usuario',
      width: 150,
    },
    {
      id: 'rol',
      label: 'Rol',
      width: 180,
      render: (row) => (
        <Chip label={row.rol} color="primary" size="small" />
      ),
    },
    {
      id: 'activo',
      label: 'Estado',
      width: 120,
      align: 'center',
      render: (row) => (
        <Badge
          text={row.activo ? 'Activo' : 'Inactivo'}
          variant={row.activo ? 'success' : 'default'}
        />
      ),
    },
    {
      id: 'fechaCreacion',
      label: 'Fecha Creación',
      width: 140,
      format: (value) => formatDate(value),
    },
    {
      id: 'ultimoAcceso',
      label: 'Último Acceso',
      width: 140,
      render: (row) => row.ultimoAcceso ? formatDate(row.ultimoAcceso) : 'Nunca',
    },
  ];

  return (
    <Box>
      {/* Header */}
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={3}
      >
        <Box>
          <Typography variant="h4" fontWeight="bold" gutterBottom>
            Gestión de Usuarios
          </Typography>
          <Typography variant="body2" color="textSecondary">
            Administra usuarios y sus roles en el sistema
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleCrearUsuario}
        >
          Nuevo Usuario
        </Button>
      </Box>

      {/* Tabla */}
      <Table
        columns={columns}
        data={usuarios}
        loading={loading}
        pagination
        page={currentPage - 1}
        pageSize={pageSize}
        totalItems={totalItems}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        emptyMessage="No hay usuarios registrados"
      />
    </Box>
  );
};