import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Box,
  Typography,
  Paper,
  Grid,
  Divider,
  IconButton,
  Stepper,
  Step,
  StepLabel,
} from '@mui/material';
import {
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon,
  CheckCircle as CheckIcon,
} from '@mui/icons-material';
import toast from 'react-hot-toast';
import { Input } from '@components/common/Input';
import { Select, SelectOption } from '@components/common/Select';
import { Button } from '@components/common/Button';
import { useAuditoriaStore } from '@store/auditoriaSlice';
import { auditoriaSchema, AuditoriaFormData } from '@utils/validators';
import { apiService } from '@services/api.service';

interface Proveedor {
  id: number;
  nombre: string;
}

interface OrdenCompra {
  id: number;
  numero: string;
}

const steps = ['Información General', 'Productos', 'Finalizar'];

export const CrearAuditoriaPage: React.FC = () => {
  const navigate = useNavigate();
  const { createAuditoria, isLoading } = useAuditoriaStore();
  const [activeStep, setActiveStep] = useState(0);
  const [proveedores, setProveedores] = useState<Proveedor[]>([]);
  const [ordenesCompra, setOrdenesCompra] = useState<OrdenCompra[]>([]);
  const [loadingData, setLoadingData] = useState(true);

  const {
    control,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<AuditoriaFormData>({
    resolver: zodResolver(auditoriaSchema),
    defaultValues: {
      proveedorId: 0,
      ordenCompraId: 0,
      fecha: new Date().toISOString().split('T')[0],
      hora: new Date().toTimeString().slice(0, 5),
      observaciones: '',
    },
  });

  const proveedorSeleccionado = watch('proveedorId');

  useEffect(() => {
    loadInitialData();
  }, []);

  useEffect(() => {
    if (proveedorSeleccionado) {
      loadOrdenesCompra(proveedorSeleccionado);
    }
  }, [proveedorSeleccionado]);

  const loadInitialData = async () => {
    setLoadingData(true);
    try {
      // Cargar proveedores
      const response = await apiService.get<any>('/proveedores', {
        params: { activo: true },
      });
      setProveedores(response.data.items || []);
    } catch (error) {
      console.error('Error al cargar datos:', error);
      toast.error('Error al cargar los datos iniciales');
    } finally {
      setLoadingData(false);
    }
  };

  const loadOrdenesCompra = async (proveedorId: number) => {
    try {
      const response = await apiService.get<any>('/ordenes-compra', {
        params: { proveedorId, estado: 'Pendiente' },
      });
      setOrdenesCompra(response.data.items || []);
    } catch (error) {
      console.error('Error al cargar órdenes de compra:', error);
    }
  };

  const onSubmit = async (data: AuditoriaFormData) => {
    try {
      const nuevaAuditoria = await createAuditoria(data);
      toast.success('Auditoría creada exitosamente');
      navigate(`/auditorias/${nuevaAuditoria.id}`);
    } catch (error: any) {
      toast.error(error.message || 'Error al crear la auditoría');
    }
  };

  const handleBack = () => {
    navigate('/auditorias');
  };

  const handleNext = () => {
    setActiveStep((prev) => prev + 1);
  };

  const handlePrevious = () => {
    setActiveStep((prev) => prev - 1);
  };

  const proveedorOptions: SelectOption[] = proveedores.map((p) => ({
    value: p.id,
    label: p.nombre,
  }));

  const ordenCompraOptions: SelectOption[] = ordenesCompra.map((oc) => ({
    value: oc.id,
    label: oc.numero,
  }));

  return (
    <Box>
      {/* Header */}
      <Box display="flex" alignItems="center" mb={3}>
        <IconButton onClick={handleBack} sx={{ mr: 2 }}>
          <ArrowBackIcon />
        </IconButton>
        <Box>
          <Typography variant="h4" fontWeight="bold">
            Nueva Auditoría de Recepción
          </Typography>
          <Typography variant="body2" color="textSecondary">
            Completa la información para crear una nueva auditoría
          </Typography>
        </Box>
      </Box>

      {/* Stepper */}
      <Box mb={4}>
        <Stepper activeStep={activeStep}>
          {steps.map((label) => (
            <Step key={label}>
              <StepLabel>{label}</StepLabel>
            </Step>
          ))}
        </Stepper>
      </Box>

      {/* Formulario */}
      <Paper sx={{ p: 4 }}>
        <form onSubmit={handleSubmit(onSubmit)}>
          {activeStep === 0 && (
            <Box>
              <Typography variant="h6" gutterBottom>
                Información General
              </Typography>
              <Divider sx={{ mb: 3 }} />

              <Grid container spacing={3}>
                <Grid item xs={12} md={6}>
                  <Controller
                    name="proveedorId"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        label="Proveedor *"
                        options={proveedorOptions}
                        error={!!errors.proveedorId}
                        errorText={errors.proveedorId?.message}
                        disabled={loadingData}
                      />
                    )}
                  />
                </Grid>

                <Grid item xs={12} md={6}>
                  <Controller
                    name="ordenCompraId"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        label="Orden de Compra *"
                        options={ordenCompraOptions}
                        error={!!errors.ordenCompraId}
                        errorText={errors.ordenCompraId?.message}
                        disabled={!proveedorSeleccionado || ordenesCompra.length === 0}
                      />
                    )}
                  />
                </Grid>

                <Grid item xs={12} md={6}>
                  <Controller
                    name="fecha"
                    control={control}
                    render={({ field }) => (
                      <Input
                        {...field}
                        label="Fecha *"
                        type="date"
                        error={!!errors.fecha}
                        errorText={errors.fecha?.message}
                        InputLabelProps={{ shrink: true }}
                      />
                    )}
                  />
                </Grid>

                <Grid item xs={12} md={6}>
                  <Controller
                    name="hora"
                    control={control}
                    render={({ field }) => (
                      <Input
                        {...field}
                        label="Hora *"
                        type="time"
                        error={!!errors.hora}
                        errorText={errors.hora?.message}
                        InputLabelProps={{ shrink: true }}
                      />
                    )}
                  />
                </Grid>

                <Grid item xs={12}>
                  <Controller
                    name="observaciones"
                    control={control}
                    render={({ field }) => (
                      <Input
                        {...field}
                        label="Observaciones"
                        multiline
                        rows={4}
                        placeholder="Ingrese observaciones adicionales (opcional)"
                        error={!!errors.observaciones}
                        errorText={errors.observaciones?.message}
                      />
                    )}
                  />
                </Grid>
              </Grid>

              <Box mt={4} display="flex" justifyContent="space-between">
                <Button variant="outlined" onClick={handleBack}>
                  Cancelar
                </Button>
                <Button variant="contained" onClick={handleNext}>
                  Siguiente
                </Button>
              </Box>
            </Box>
          )}

          {activeStep === 1 && (
            <Box>
              <Typography variant="h6" gutterBottom>
                Agregar Productos
              </Typography>
              <Divider sx={{ mb: 3 }} />

              <Box
                display="flex"
                flexDirection="column"
                alignItems="center"
                justifyContent="center"
                minHeight={300}
                sx={{
                  border: '2px dashed',
                  borderColor: 'grey.300',
                  borderRadius: 2,
                  p: 4,
                }}
              >
                <Typography variant="h6" color="textSecondary" gutterBottom>
                  Agrega productos después de crear la auditoría
                </Typography>
                <Typography variant="body2" color="textSecondary" align="center">
                  Podrás escanear productos o agregarlos manualmente en la siguiente pantalla
                </Typography>
              </Box>

              <Box mt={4} display="flex" justifyContent="space-between">
                <Button variant="outlined" onClick={handlePrevious}>
                  Anterior
                </Button>
                <Button variant="contained" onClick={handleNext}>
                  Siguiente
                </Button>
              </Box>
            </Box>
          )}

          {activeStep === 2 && (
            <Box>
              <Typography variant="h6" gutterBottom>
                Resumen
              </Typography>
              <Divider sx={{ mb: 3 }} />

              <Box
                display="flex"
                flexDirection="column"
                alignItems="center"
                justifyContent="center"
                minHeight={300}
              >
                <CheckIcon sx={{ fontSize: 80, color: 'success.main', mb: 2 }} />
                <Typography variant="h5" gutterBottom>
                  ¿Crear Auditoría?
                </Typography>
                <Typography variant="body2" color="textSecondary" align="center">
                  Se creará una nueva auditoría con la información proporcionada.
                  Podrás agregar productos inmediatamente después.
                </Typography>
              </Box>

              <Box mt={4} display="flex" justifyContent="space-between">
                <Button variant="outlined" onClick={handlePrevious}>
                  Anterior
                </Button>
                <Button
                  type="submit"
                  variant="contained"
                  loading={isLoading}
                  icon={<SaveIcon />}
                >
                  Crear Auditoría
                </Button>
              </Box>
            </Box>
          )}
        </form>
      </Paper>
    </Box>
  );
};