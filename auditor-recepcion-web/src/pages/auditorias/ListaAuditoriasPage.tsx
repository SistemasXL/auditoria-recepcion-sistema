import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Button,
  TextField,
  InputAdornment,
  Chip,
} from '@mui/material';
import {
  Add as AddIcon,
  Search as SearchIcon,
  QrCodeScanner as ScanIcon,
} from '@mui/icons-material';
import { Table, Column } from '@components/common/Table';
import { Badge } from '@components/common/Badge';
import { useAuditoriaStore } from '@store/auditoriaSlice';
import { Auditoria, EstadoAuditoria } from '@types/auditoria.types';
import { formatDate, formatAuditoriaNumber } from '@utils/formatters';
import { ESTADOS_AUDITORIA_CONFIG } from '@config/constants';

export const ListaAuditoriasPage: React.FC = () => {
  const navigate = useNavigate();
  const {
    auditorias,
    totalItems,
    currentPage,
    isLoading,
    fetchAuditorias,
  } = useAuditoriaStore();

  const [searchTerm, setSearchTerm] = useState('');
  const [pageSize, setPageSize] = useState(10);

  useEffect(() => {
    loadAuditorias();
  }, [currentPage, pageSize]);

  const loadAuditorias = () => {
    const filters = searchTerm ? { search: searchTerm } : undefined;
    fetchAuditorias(currentPage, pageSize, filters);
  };

  const handleSearch = () => {
    fetchAuditorias(1, pageSize, { search: searchTerm });
  };

  const handlePageChange = (newPage: number) => {
    fetchAuditorias(newPage + 1, pageSize);
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize);
    fetchAuditorias(1, newPageSize);
  };

  const handleRowClick = (auditoria: Auditoria) => {
    navigate(`/auditorias/${auditoria.id}`);
  };

  const handleCrearAuditoria = () => {
    navigate('/auditorias/crear');
  };

  const handleEscanear = () => {
    navigate('/escanear');
  };

  const getEstadoBadge = (estado: EstadoAuditoria) => {
    const config = ESTADOS_AUDITORIA_CONFIG[estado];
    const variantMap: Record<string, 'success' | 'error' | 'warning' | 'info' | 'default'> = {
      [EstadoAuditoria.FINALIZADA]: 'success',
      [EstadoAuditoria.EN_PROCESO]: 'info',
      [EstadoAuditoria.CANCELADA]: 'error',
      [EstadoAuditoria.BORRADOR]: 'default',
      [EstadoAuditoria.CERRADA]: 'default',
    };

    return (
      <Badge
        text={config?.label || estado}
        variant={variantMap[estado] || 'default'}
      />
    );
  };

  const columns: Column<Auditoria>[] = [
    {
      id: 'numeroAuditoria',
      label: 'N° Auditoría',
      width: 150,
      render: (row) => (
        <Typography variant="body2" fontWeight="medium">
          {row.numeroAuditoria}
        </Typography>
      ),
    },
    {
      id: 'fecha',
      label: 'Fecha',
      width: 120,
      format: (value) => formatDate(value),
    },
    {
      id: 'proveedor',
      label: 'Proveedor',
      render: (row) => row.proveedor.nombre,
    },
    {
      id: 'ordenCompra',
      label: 'Orden de Compra',
      width: 150,
    },
    {
      id: 'totalProductos',
      label: 'Productos',
      width: 100,
      align: 'center',
    },
    {
      id: 'estado',
      label: 'Estado',
      width: 140,
      align: 'center',
      render: (row) => getEstadoBadge(row.estado),
    },
    {
      id: 'tieneIncidencias',
      label: 'Incidencias',
      width: 120,
      align: 'center',
      render: (row) => (
        row.tieneIncidencias ? (
          <Chip label="Sí" color="warning" size="small" />
        ) : (
          <Chip label="No" size="small" />
        )
      ),
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
            Auditorías de Recepción
          </Typography>
          <Typography variant="body2" color="textSecondary">
            Gestiona y consulta todas las auditorías de recepción de productos
          </Typography>
        </Box>
        <Box display="flex" gap={2}>
          <Button
            variant="outlined"
            startIcon={<ScanIcon />}
            onClick={handleEscanear}
          >
            Escanear
          </Button>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={handleCrearAuditoria}
          >
            Nueva Auditoría
          </Button>
        </Box>
      </Box>

      {/* Filtros y búsqueda */}
      <Box mb={3}>
        <TextField
          placeholder="Buscar por N° auditoría, proveedor u orden de compra..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
          fullWidth
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            ),
          }}
        />
      </Box>

      {/* Tabla */}
      <Table
        columns={columns}
        data={auditorias}
        loading={isLoading}
        pagination
        page={currentPage - 1}
        pageSize={pageSize}
        totalItems={totalItems}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        onRowClick={handleRowClick}
        emptyMessage="No se encontraron auditorías"
      />
    </Box>
  );
};