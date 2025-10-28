import React, { useEffect, useState } from 'react';
import {
  Box,
  Typography,
  TextField,
  InputAdornment,
} from '@mui/material';
import {
  Search as SearchIcon,
} from '@mui/icons-material';
import { Table, Column } from '@components/common/Table';
import { Badge } from '@components/common/Badge';
import { incidenciasService } from '@services/incidencias.service';
import { Incidencia, SeveridadIncidencia, EstadoIncidencia } from '@types/incidencia.types';
import { formatDate } from '@utils/formatters';
import { SEVERIDAD_INCIDENCIA_CONFIG, ESTADOS_INCIDENCIA_CONFIG } from '@config/constants';
import toast from 'react-hot-toast';

export const ListaIncidenciasPage: React.FC = () => {
  const [incidencias, setIncidencias] = useState<Incidencia[]>([]);
  const [loading, setLoading] = useState(false);
  const [totalItems, setTotalItems] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadIncidencias();
  }, [currentPage, pageSize]);

  const loadIncidencias = async () => {
    setLoading(true);
    try {
      const filters = searchTerm ? { search: searchTerm } : undefined;
      const response = await incidenciasService.getIncidencias(
        { page: currentPage, pageSize },
        filters
      );
      setIncidencias(response.items);
      setTotalItems(response.totalItems);
    } catch (error) {
      console.error('Error al cargar incidencias:', error);
      toast.error('Error al cargar incidencias');
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    setCurrentPage(1);
    loadIncidencias();
  };

  const handlePageChange = (newPage: number) => {
    setCurrentPage(newPage + 1);
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize);
    setCurrentPage(1);
  };

  const getSeveridadBadge = (severidad: SeveridadIncidencia) => {
    const variantMap: Record<SeveridadIncidencia, 'success' | 'warning' | 'error'> = {
      [SeveridadIncidencia.BAJA]: 'success',
      [SeveridadIncidencia.MEDIA]: 'warning',
      [SeveridadIncidencia.ALTA]: 'error',
      [SeveridadIncidencia.CRITICA]: 'error',
    };

    return (
      <Badge
        text={SEVERIDAD_INCIDENCIA_CONFIG[severidad].label}
        variant={variantMap[severidad]}
      />
    );
  };

  const getEstadoBadge = (estado: EstadoIncidencia) => {
    const variantMap: Record<EstadoIncidencia, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
      [EstadoIncidencia.ABIERTA]: 'error',
      [EstadoIncidencia.EN_REVISION]: 'warning',
      [EstadoIncidencia.RESUELTA]: 'success',
      [EstadoIncidencia.CERRADA]: 'default',
      [EstadoIncidencia.RECHAZADA]: 'default',
    };

    return (
      <Badge
        text={ESTADOS_INCIDENCIA_CONFIG[estado].label}
        variant={variantMap[estado]}
      />
    );
  };

  const columns: Column<Incidencia>[] = [
    {
      id: 'numeroIncidencia',
      label: 'N° Incidencia',
      width: 150,
      render: (row) => (
        <Typography variant="body2" fontWeight="medium">
          {row.numeroIncidencia}
        </Typography>
      ),
    },
    {
      id: 'auditoria',
      label: 'Auditoría',
      width: 150,
      render: (row) => row.auditoria.numeroAuditoria,
    },
    {
      id: 'tipo',
      label: 'Tipo',
      width: 180,
    },
    {
      id: 'severidad',
      label: 'Severidad',
      width: 120,
      align: 'center',
      render: (row) => getSeveridadBadge(row.severidad),
    },
    {
      id: 'descripcion',
      label: 'Descripción',
      render: (row) => (
        <Typography
          variant="body2"
          sx={{
            maxWidth: 300,
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
          }}
        >
          {row.descripcion}
        </Typography>
      ),
    },
    {
      id: 'estado',
      label: 'Estado',
      width: 140,
      align: 'center',
      render: (row) => getEstadoBadge(row.estado),
    },
    {
      id: 'fechaCreacion',
      label: 'Fecha',
      width: 120,
      format: (value) => formatDate(value),
    },
  ];

  return (
    <Box>
      {/* Header */}
      <Box mb={3}>
        <Typography variant="h4" fontWeight="bold" gutterBottom>
          Incidencias
        </Typography>
        <Typography variant="body2" color="textSecondary">
          Gestión de incidencias en auditorías de recepción
        </Typography>
      </Box>

      {/* Búsqueda */}
      <Box mb={3}>
        <TextField
          placeholder="Buscar por N° incidencia, auditoría o descripción..."
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
        data={incidencias}
        loading={loading}
        pagination
        page={currentPage - 1}
        pageSize={pageSize}
        totalItems={totalItems}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        emptyMessage="No se encontraron incidencias"
      />
    </Box>
  );
};