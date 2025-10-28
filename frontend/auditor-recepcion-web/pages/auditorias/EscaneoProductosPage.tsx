import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import {
  Box,
  Typography,
  Paper,
  Grid,
  IconButton,
  TextField,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  Divider,
} from '@mui/material';
import {
  ArrowBack as ArrowBackIcon,
  QrCodeScanner as ScanIcon,
  Delete as DeleteIcon,
  Add as AddIcon,
  Check as CheckIcon,
} from '@mui/icons-material';
import toast from 'react-hot-toast';
import { Button } from '@components/common/Button';
import { Input } from '@components/common/Input';
import { Card } from '@components/common/Card';
import { useScanner } from '@hooks/useScanner';
import { useAuditoriaStore } from '@store/auditoriaSlice';
import { ProductoInfo } from '@types/auditoria.types';

interface ProductoEscaneado {
  producto: ProductoInfo;
  cantidadEsperada: number;
  cantidadRecibida: number;
}

export const EscaneoProductosPage: React.FC = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const auditoriaId = searchParams.get('auditoriaId');
  
  const { agregarProducto } = useAuditoriaStore();
  const [scanning, setScanning] = useState(false);
  const [productosEscaneados, setProductosEscaneados] = useState<ProductoEscaneado[]>([]);
  const [codigoManual, setCodigoManual] = useState('');
  const [cantidadEsperada, setCantidadEsperada] = useState('');
  const [cantidadRecibida, setCantidadRecibida] = useState('');

  const {
    isScanning,
    producto,
    startScanner,
    stopScanner,
    searchByCode,
    reset,
  } = useScanner({
    onScanSuccess: (productoEncontrado) => {
      toast.success(`Producto escaneado: ${productoEncontrado.nombre}`);
      setScanning(false);
      stopScanner();
    },
    onScanError: (error) => {
      toast.error(error);
    },
  });

  useEffect(() => {
    if (!auditoriaId) {
      toast.error('No se especificó una auditoría');
      navigate('/auditorias');
    }
  }, [auditoriaId, navigate]);

  const handleStartScan = async () => {
    setScanning(true);
    try {
      await startScanner('scanner-container');
    } catch (error) {
      console.error('Error al iniciar escáner:', error);
      setScanning(false);
    }
  };

  const handleStopScan = () => {
    setScanning(false);
    stopScanner();
  };

  const handleBuscarManual = async () => {
    if (!codigoManual) {
      toast.error('Ingrese un código de barras');
      return;
    }

    try {
      await searchByCode(codigoManual);
      setCodigoManual('');
    } catch (error) {
      toast.error('Producto no encontrado');
    }
  };

  const handleAgregarProducto = () => {
    if (!producto) {
      toast.error('No hay producto seleccionado');
      return;
    }

    if (!cantidadEsperada || !cantidadRecibida) {
      toast.error('Ingrese las cantidades');
      return;
    }

    const nuevoProducto: ProductoEscaneado = {
      producto,
      cantidadEsperada: parseInt(cantidadEsperada),
      cantidadRecibida: parseInt(cantidadRecibida),
    };

    setProductosEscaneados([...productosEscaneados, nuevoProducto]);
    
    // Limpiar formulario
    reset();
    setCantidadEsperada('');
    setCantidadRecibida('');
    toast.success('Producto agregado a la lista');
  };

  const handleEliminarProducto = (index: number) => {
    setProductosEscaneados(productosEscaneados.filter((_, i) => i !== index));
    toast.success('Producto eliminado');
  };

  const handleGuardarTodos = async () => {
    if (productosEscaneados.length === 0) {
      toast.error('No hay productos para guardar');
      return;
    }

    if (!auditoriaId) return;

    try {
      for (const item of productosEscaneados) {
        await agregarProducto({
          auditoriaId: parseInt(auditoriaId),
          productoId: item.producto.id,
          cantidadEsperada: item.cantidadEsperada,
          cantidadRecibida: item.cantidadRecibida,
        });
      }

      toast.success('Productos guardados exitosamente');
      navigate(`/auditorias/${auditoriaId}`);
    } catch (error: any) {
      toast.error(error.message || 'Error al guardar productos');
    }
  };

  const handleBack = () => {
    if (auditoriaId) {
      navigate(`/auditorias/${auditoriaId}`);
    } else {
      navigate('/auditorias');
    }
  };

  return (
    <Box>
      {/* Header */}
      <Box display="flex" alignItems="center" mb={3}>
        <IconButton onClick={handleBack} sx={{ mr: 2 }}>
          <ArrowBackIcon />
        </IconButton>
        <Box>
          <Typography variant="h4" fontWeight="bold">
            Escanear Productos
          </Typography>
          <Typography variant="body2" color="textSecondary">
            Escanea códigos de barras o ingresa manualmente
          </Typography>
        </Box>
      </Box>

      <Grid container spacing={3}>
        {/* Scanner */}
        <Grid item xs={12} md={6}>
          <Card title="Escáner de Código de Barras">
            {!scanning ? (
              <Box textAlign="center" py={4}>
                <Button
                  variant="contained"
                  size="large"
                  startIcon={<ScanIcon />}
                  onClick={handleStartScan}
                >
                  Iniciar Escáner
                </Button>
              </Box>
            ) : (
              <Box>
                <Box
                  id="scanner-container"
                  sx={{
                    width: '100%',
                    height: 300,
                    backgroundColor: 'black',
                    borderRadius: 2,
                    mb: 2,
                  }}
                />
                <Button
                  variant="outlined"
                  fullWidth
                  onClick={handleStopScan}
                >
                  Detener Escáner
                </Button>
              </Box>
            )}
          </Card>

          <Card title="Búsqueda Manual" sx={{ mt: 3 }}>
            <Box display="flex" gap={2}>
              <TextField
                label="Código de Barras"
                value={codigoManual}
                onChange={(e) => setCodigoManual(e.target.value)}
                onKeyPress={(e) => e.key === 'Enter' && handleBuscarManual()}
                fullWidth
              />
              <Button onClick={handleBuscarManual}>
                Buscar
              </Button>
            </Box>
          </Card>
        </Grid>

        {/* Producto actual */}
        <Grid item xs={12} md={6}>
          <Card title="Producto Escaneado">
            {producto ? (
              <Box>
                <Paper sx={{ p: 2, mb: 3, backgroundColor: 'grey.50' }}>
                  <Typography variant="h6" gutterBottom>
                    {producto.nombre}
                  </Typography>
                  <Typography variant="body2" color="textSecondary">
                    SKU: {producto.sku}
                  </Typography>
                  <Typography variant="body2" color="textSecondary">
                    Código: {producto.codigoBarras}
                  </Typography>
                </Paper>

                <Grid container spacing={2}>
                  <Grid item xs={6}>
                    <Input
                      label="Cantidad Esperada"
                      type="number"
                      value={cantidadEsperada}
                      onChange={(e) => setCantidadEsperada(e.target.value)}
                      fullWidth
                    />
                  </Grid>
                  <Grid item xs={6}>
                    <Input
                      label="Cantidad Recibida"
                      type="number"
                      value={cantidadRecibida}
                      onChange={(e) => setCantidadRecibida(e.target.value)}
                      fullWidth
                    />
                  </Grid>
                </Grid>

                <Button
                  variant="contained"
                  startIcon={<AddIcon />}
                  onClick={handleAgregarProducto}
                  fullWidth
                  sx={{ mt: 2 }}
                >
                  Agregar a la Lista
                </Button>
              </Box>
            ) : (
              <Box textAlign="center" py={4}>
                <Typography color="textSecondary">
                  Escanea o busca un producto para comenzar
                </Typography>
              </Box>
            )}
          </Card>
        </Grid>

        {/* Lista de productos */}
        <Grid item xs={12}>
          <Card
            title={`Productos Agregados (${productosEscaneados.length})`}
            actions={
              productosEscaneados.length > 0 && (
                <Button
                  variant="contained"
                  color="success"
                  startIcon={<CheckIcon />}
                  onClick={handleGuardarTodos}
                >
                  Guardar Todos
                </Button>
              )
            }
          >
            {productosEscaneados.length === 0 ? (
              <Box textAlign="center" py={4}>
                <Typography color="textSecondary">
                  No hay productos agregados
                </Typography>
              </Box>
            ) : (
              <List>
                {productosEscaneados.map((item, index) => (
                  <React.Fragment key={index}>
                    <ListItem>
                      <ListItemText
                        primary={item.producto.nombre}
                        secondary={
                          <Box component="span">
                            SKU: {item.producto.sku} | 
                            Esperado: {item.cantidadEsperada} | 
                            Recibido: {item.cantidadRecibida}
                          </Box>
                        }
                      />
                      <ListItemSecondaryAction>
                        <IconButton
                          edge="end"
                          onClick={() => handleEliminarProducto(index)}
                        >
                          <DeleteIcon />
                        </IconButton>
                      </ListItemSecondaryAction>
                    </ListItem>
                    {index < productosEscaneados.length - 1 && <Divider />}
                  </React.Fragment>
                ))}
              </List>
            )}
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};