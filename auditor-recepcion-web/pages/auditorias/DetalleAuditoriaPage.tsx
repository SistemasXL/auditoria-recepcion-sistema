import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Paper,
  Grid,
  Chip,
  IconButton,
  Tabs,
  Tab,
  Divider,
} from '@mui/material';
import {
  ArrowBack as ArrowBackIcon,
  Edit as EditIcon,
  CheckCircle as CheckIcon,
  Lock as LockIcon,
} from '@mui/icons-material';
import toast from 'react-hot-toast';
import { Badge } from '@components/common/Badge';
import { Button } from '@components/common/Button';
import { Loader } from '@components/common/Loader';
import { Table, Column } from '@components/common/Table';
import { useAuditoriaStore } from '@store/auditoriaSlice';
import { ProductoAuditoria } from '@types/auditoria.types';
import { formatDate, formatDateTime } from '@utils/formatters';
import { ESTADOS_AUDITORIA_CONFIG } from '@config/constants';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;
  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`tabpanel-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ py: 3 }}>{children}</Box>}
    </div>
  );
}

export const DetalleAuditoriaPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const {
    auditoriaActual,
    productosAuditoria,
    evidencias,
    isLoading,
    fetchAuditoriaById,
    finalizarAuditoria,
    cerrarAuditoria,
  } = useAuditoriaStore();

  const [tabValue, setTabValue] = useState(0);
  const [finalizando, setFinalizando] = useState(false);

  useEffect(() => {
    if (id) {
      fetchAuditoriaById(parseInt(id));
    }
  }, [id]);

  const handleBack = () => {
    navigate('/auditorias');
  };

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const handleFinalizar = async () => {
    if (!auditoriaActual) return;

    if (productosAuditoria.length === 0) {
      toast.error('Debe agregar al menos un producto antes de finalizar');
      return;
    }

    const confirmar = window.confirm('¿Está seguro que desea finalizar esta auditoría?');
    if (!confirmar) return;

    setFinalizando(true);
    try {
      await finalizarAuditoria(auditoriaActual.id);
      toast.success('Auditoría finalizada exitosamente');
    } catch (error: any) {
      toast.error(error.message || 'Error al finalizar la auditoría');
    } finally {
      setFinalizando(false);
    }
  };

  const handleCerrar = async () => {
    if (!auditoriaActual) return;

    const confirmar = window.confirm('¿Está seguro que desea cerrar esta auditoría?');
    if (!confirmar) return;

    try {
      await cerrarAuditoria(auditoriaActual.id);
      toast.success('Auditoría cerrada exitosamente');
    } catch (error: any) {
      toast.error(error.message || 'Error al cerrar la auditoría');
    }
  };

  const handleAgregarProductos = () => {
    navigate(`/escanear?auditoriaId=${id}`);
  };

  if (isLoading || !auditoriaActual) {
    return <Loader fullScreen message="Cargando auditoría..." />;
  }

  const estadoConfig = ESTADOS_AUDITORIA_CONFIG[auditoriaActual.estado];

  const columnasProductos: Column<ProductoAuditoria>[] = [
    {
      id: 'producto',
      label: 'Producto',
      render: (row) => (
        <Box>
          <Typography variant="body2" fontWeight="medium">
            {row.producto.nombre}
          </Typography>
          <Typography variant="caption" color="textSecondary">
            SKU: {row.producto.sku}
          </Typography>
        </Box>
      ),
    },
    {
      id: 'cantidadEsperada',
      label: 'Esperado',
      align: 'center',
      width: 100,
    },
    {
      id: 'cantidadRecibida',
      label: 'Recibido',
      align: 'center',
      width: 100,
    },
    {
      id: 'cantidadDiferencia',
      label: 'Diferencia',
      align: 'center',
      width: 120,
      render: (row) => {
        const diff = row.cantidadDiferencia;
        const color = diff === 0 ? 'success' : diff > 0 ? 'info' : 'error';
        return (
          <Chip
            label={diff > 0 ? `+${diff}` : diff}
            color={color}
            size="small"
          />
        );
      },
    },
    {
      id: 'observaciones',
      label: 'Observaciones',
      render: (row) => row.observaciones || '-',
    },
  ];

  return (
    <Box>
      {/* Header */}
      <Box display="flex" alignItems="center" justifyContent="space-between" mb={3}>
        <Box display="flex" alignItems="center">
          <IconButton onClick={handleBack} sx={{ mr: 2 }}>
            <ArrowBackIcon />
          </IconButton>
          <Box>
            <Box display="flex" alignItems="center" gap={2}>
              <Typography variant="h4" fontWeight="bold">
                {auditoriaActual.numeroAuditoria}
              </Typography>
              <Badge
                text={estadoConfig.label}
                variant={
                  auditoriaActual.estado === 'Finalizada'
                    ? 'success'
                    : auditoriaActual.estado === 'En Proceso'
                    ? 'info'
                    : 'default'
                }
              />
            </Box>
            <Typography variant="body2" color="textSecondary">
              Creada: {formatDateTime(auditoriaActual.fechaCreacion)}
            </Typography>
          </Box>
        </Box>

        <Box display="flex" gap={2}>
          {auditoriaActual.estado === 'Borrador' && (
            <Button
              variant="outlined"
              startIcon={<EditIcon />}
              onClick={() => navigate(`/auditorias/${id}/editar`)}
            >
              Editar
            </Button>
          )}
          {auditoriaActual.estado === 'En Proceso' && (
            <Button
              variant="contained"
              startIcon={<CheckIcon />}
              onClick={handleFinalizar}
              loading={finalizando}
            >
              Finalizar
            </Button>
          )}
          {auditoriaActual.estado === 'Finalizada' && (
            <Button
              variant="contained"
              color="secondary"
              startIcon={<LockIcon />}
              onClick={handleCerrar}
            >
              Cerrar
            </Button>
          )}
        </Box>
      </Box>

      {/* Información general */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            <Typography variant="caption" color="textSecondary">
              Proveedor
            </Typography>
            <Typography variant="body1" fontWeight="medium">
              {auditoriaActual.proveedor.nombre}
            </Typography>
          </Grid>
          <Grid item xs={12} md={6}>
            <Typography variant="caption" color="textSecondary">
              Orden de Compra
            </Typography>
            <Typography variant="body1" fontWeight="medium">
              {auditoriaActual.ordenCompra}
            </Typography>
          </Grid>
          <Grid item xs={12} md={4}>
            <Typography variant="caption" color="textSecondary">
              Fecha de Recepción
            </Typography>
            <Typography variant="body1" fontWeight="medium">
              {formatDate(auditoriaActual.fecha)} - {auditoriaActual.hora}
            </Typography>
          </Grid>
          <Grid item xs={12} md={4}>
            <Typography variant="caption" color="textSecondary">
              Total Productos
            </Typography>
            <Typography variant="body1" fontWeight="medium">
              {auditoriaActual.totalProductos}
            </Typography>
          </Grid>
          <Grid item xs={12} md={4}>
            <Typography variant="caption" color="textSecondary">
              Usuario Creador
            </Typography>
            <Typography variant="body1" fontWeight="medium">
              {auditoriaActual.usuarioCreador}
            </Typography>
          </Grid>
          {auditoriaActual.observaciones && (
            <Grid item xs={12}>
              <Typography variant="caption" color="textSecondary">
                Observaciones
              </Typography>
              <Typography variant="body1">
                {auditoriaActual.observaciones}
              </Typography>
            </Grid>
          )}
        </Grid>
      </Paper>

      {/* Tabs */}
      <Paper>
        <Tabs value={tabValue} onChange={handleTabChange}>
          <Tab label={`Productos (${productosAuditoria.length})`} />
          <Tab label={`Evidencias (${evidencias.length})`} />
          <Tab label={`Incidencias (${auditoriaActual.tieneIncidencias ? '!' : '0'})`} />
        </Tabs>

        <Divider />

        {/* Tab: Productos */}
        <TabPanel value={tabValue} index={0}>
          {productosAuditoria.length === 0 ? (
            <Box textAlign="center" py={4}>
              <Typography color="textSecondary" gutterBottom>
                No hay productos agregados
              </Typography>
              {auditoriaActual.estado !== 'Cerrada' && (
                <Button
                  variant="contained"
                  onClick={handleAgregarProductos}
                  sx={{ mt: 2 }}
                >
                  Agregar Productos
                </Button>
              )}
            </Box>
          ) : (
            <Table
              columns={columnasProductos}
              data={productosAuditoria}
              pagination={false}
            />
          )}
        </TabPanel>

        {/* Tab: Evidencias */}
        <TabPanel value={tabValue} index={1}>
          <Box textAlign="center" py={4}>
            <Typography color="textSecondary">
              Galería de evidencias (implementar)
            </Typography>
          </Box>
        </TabPanel>

        {/* Tab: Incidencias */}
        <TabPanel value={tabValue} index={2}>
          <Box textAlign="center" py={4}>
            <Typography color="textSecondary">
              Lista de incidencias (implementar)
            </Typography>
          </Box>
        </TabPanel>
      </Paper>
    </Box>
  );
};