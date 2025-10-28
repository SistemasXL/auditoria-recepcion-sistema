import React, { useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  Grid,
  Tabs,
  Tab,
  TextField,
  MenuItem,
} from '@mui/material';
import {
  PictureAsPdf as PdfIcon,
  TableChart as ExcelIcon,
} from '@mui/icons-material';
import { Button } from '@components/common/Button';
import { reportesService } from '@services/reportes.service';
import toast from 'react-hot-toast';

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

export const ReportesPage: React.FC = () => {
  const [tabValue, setTabValue] = useState(0);
  const [loading, setLoading] = useState(false);

  // Filtros comunes
  const [fechaDesde, setFechaDesde] = useState('');
  const [fechaHasta, setFechaHasta] = useState('');

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const handleExportarPDF = async (tipo: 'auditorias' | 'incidencias' | 'proveedores') => {
    setLoading(true);
    try {
      const params = {
        dateFrom: fechaDesde,
        dateTo: fechaHasta,
      };
      
      await reportesService.exportarPDF(tipo, params);
      toast.success('Reporte descargado exitosamente');
    } catch (error) {
      console.error('Error al exportar PDF:', error);
      toast.error('Error al generar el reporte');
    } finally {
      setLoading(false);
    }
  };

  const handleExportarExcel = async (tipo: 'auditorias' | 'incidencias' | 'proveedores') => {
    setLoading(true);
    try {
      const params = {
        dateFrom: fechaDesde,
        dateTo: fechaHasta,
      };
      
      await reportesService.exportarExcel(tipo, params);
      toast.success('Reporte descargado exitosamente');
    } catch (error) {
      console.error('Error al exportar Excel:', error);
      toast.error('Error al generar el reporte');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box>
      {/* Header */}
      <Box mb={3}>
        <Typography variant="h4" fontWeight="bold" gutterBottom>
          Reportes
        </Typography>
        <Typography variant="body2" color="textSecondary">
          Genera y descarga reportes en PDF o Excel
        </Typography>
      </Box>

      {/* Filtros globales */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Filtros
        </Typography>
        <Grid container spacing={2}>
          <Grid item xs={12} md={6}>
            <TextField
              label="Fecha Desde"
              type="date"
              value={fechaDesde}
              onChange={(e) => setFechaDesde(e.target.value)}
              fullWidth
              InputLabelProps={{ shrink: true }}
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              label="Fecha Hasta"
              type="date"
              value={fechaHasta}
              onChange={(e) => setFechaHasta(e.target.value)}
              fullWidth
              InputLabelProps={{ shrink: true }}
            />
          </Grid>
        </Grid>
      </Paper>

      {/* Tabs de reportes */}
      <Paper>
        <Tabs value={tabValue} onChange={handleTabChange}>
          <Tab label="Auditorías" />
          <Tab label="Incidencias" />
          <Tab label="Proveedores" />
        </Tabs>

        {/* Tab: Auditorías */}
        <TabPanel value={tabValue} index={0}>
          <Box p={3}>
            <Typography variant="h6" gutterBottom>
              Reporte de Auditorías
            </Typography>
            <Typography variant="body2" color="textSecondary" paragraph>
              Genera un reporte completo de todas las auditorías en el período seleccionado,
              incluyendo productos, estados, y estadísticas.
            </Typography>

            <Box display="flex" gap={2} mt={3}>
              <Button
                variant="contained"
                startIcon={<PdfIcon />}
                onClick={() => handleExportarPDF('auditorias')}
                loading={loading}
              >
                Descargar PDF
              </Button>
              <Button
                variant="outlined"
                startIcon={<ExcelIcon />}
                onClick={() => handleExportarExcel('auditorias')}
                loading={loading}
              >
                Descargar Excel
              </Button>
            </Box>
          </Box>
        </TabPanel>

        {/* Tab: Incidencias */}
        <TabPanel value={tabValue} index={1}>
          <Box p={3}>
            <Typography variant="h6" gutterBottom>
              Reporte de Incidencias
            </Typography>
            <Typography variant="body2" color="textSecondary" paragraph>
              Genera un reporte de todas las incidencias registradas, incluyendo tipo,
              severidad, estado y acciones correctivas.
            </Typography>

            <Box display="flex" gap={2} mt={3}>
              <Button
                variant="contained"
                startIcon={<PdfIcon />}
                onClick={() => handleExportarPDF('incidencias')}
                loading={loading}
              >
                Descargar PDF
              </Button>
              <Button
                variant="outlined"
                startIcon={<ExcelIcon />}
                onClick={() => handleExportarExcel('incidencias')}
                loading={loading}
              >
                Descargar Excel
              </Button>
            </Box>
          </Box>
        </TabPanel>

        {/* Tab: Proveedores */}
        <TabPanel value={tabValue} index={2}>
          <Box p={3}>
            <Typography variant="h6" gutterBottom>
              Reporte de Proveedores
            </Typography>
            <Typography variant="body2" color="textSecondary" paragraph>
              Genera un reporte de desempeño de proveedores, incluyendo tasa de precisión,
              incidencias y auditorías realizadas.
            </Typography>

            <Box display="flex" gap={2} mt={3}>
              <Button
                variant="contained"
                startIcon={<PdfIcon />}
                onClick={() => handleExportarPDF('proveedores')}
                loading={loading}
              >
                Descargar PDF
              </Button>
              <Button
                variant="outlined"
                startIcon={<ExcelIcon />}
                onClick={() => handleExportarExcel('proveedores')}
                loading={loading}
              >
                Descargar Excel
              </Button>
            </Box>
          </Box>
        </TabPanel>
      </Paper>
    </Box>
  );
};