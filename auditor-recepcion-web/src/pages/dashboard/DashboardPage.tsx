import React, { useEffect, useState } from 'react';
import {
  Box,
  Grid,
  Typography,
  Paper,
  Card,
  CardContent,
} from '@mui/material';
import {
  TrendingUp,
  Assignment,
  CheckCircle,
  Warning,
} from '@mui/icons-material';
import { auditoriasService } from '@services/auditorias.service';
import { incidenciasService } from '@services/incidencias.service';
import { Loader } from '@components/common/Loader';

interface KPICardProps {
  title: string;
  value: string | number;
  icon: React.ReactNode;
  color: string;
  trend?: string;
}

const KPICard: React.FC<KPICardProps> = ({ title, value, icon, color, trend }) => {
  return (
    <Card elevation={2}>
      <CardContent>
        <Box display="flex" justifyContent="space-between" alignItems="flex-start">
          <Box>
            <Typography color="textSecondary" variant="body2" gutterBottom>
              {title}
            </Typography>
            <Typography variant="h4" fontWeight="bold" sx={{ mb: 1 }}>
              {value}
            </Typography>
            {trend && (
              <Typography variant="caption" color="success.main">
                {trend}
              </Typography>
            )}
          </Box>
          <Box
            sx={{
              backgroundColor: color,
              borderRadius: 2,
              p: 1.5,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
            }}
          >
            {icon}
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
};

export const DashboardPage: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [estadisticas, setEstadisticas] = useState<any>(null);

  useEffect(() => {
    loadEstadisticas();
  }, []);

  const loadEstadisticas = async () => {
    setLoading(true);
    try {
      // Cargar estadísticas de auditorías e incidencias
      const [statsAuditorias, statsIncidencias] = await Promise.all([
        auditoriasService.getEstadisticas(),
        incidenciasService.getEstadisticas(),
      ]);

      setEstadisticas({
        auditorias: statsAuditorias,
        incidencias: statsIncidencias,
      });
    } catch (error) {
      console.error('Error al cargar estadísticas:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <Loader fullScreen message="Cargando dashboard..." />;
  }

  return (
    <Box>
      <Typography variant="h4" fontWeight="bold" gutterBottom>
        Dashboard
      </Typography>
      <Typography variant="body2" color="textSecondary" sx={{ mb: 4 }}>
        Resumen de auditorías y operaciones
      </Typography>

      <Grid container spacing={3}>
        {/* KPIs */}
        <Grid item xs={12} sm={6} md={3}>
          <KPICard
            title="Auditorías del Mes"
            value={estadisticas?.auditorias?.totalMes || 0}
            icon={<Assignment sx={{ color: 'white', fontSize: 32 }} />}
            color="#2563eb"
            trend="+12% vs mes anterior"
          />
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <KPICard
            title="Auditorías Pendientes"
            value={estadisticas?.auditorias?.pendientes || 0}
            icon={<TrendingUp sx={{ color: 'white', fontSize: 32 }} />}
            color="#f59e0b"
          />
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <KPICard
            title="Tasa de Precisión"
            value={`${estadisticas?.auditorias?.tasaPrecision || 98.5}%`}
            icon={<CheckCircle sx={{ color: 'white', fontSize: 32 }} />}
            color="#10b981"
            trend="+2.3% vs mes anterior"
          />
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <KPICard
            title="Incidencias Abiertas"
            value={estadisticas?.incidencias?.totalIncidencias || 0}
            icon={<Warning sx={{ color: 'white', fontSize: 32 }} />}
            color="#ef4444"
          />
        </Grid>

        {/* Gráficos y tablas */}
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3, height: 400 }}>
            <Typography variant="h6" gutterBottom>
              Tendencia de Auditorías
            </Typography>
            <Box
              display="flex"
              alignItems="center"
              justifyContent="center"
              height="90%"
            >
              <Typography color="textSecondary">
                Gráfico de tendencias (implementar con Recharts)
              </Typography>
            </Box>
          </Paper>
        </Grid>

        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 3, height: 400 }}>
            <Typography variant="h6" gutterBottom>
              Incidencias por Tipo
            </Typography>
            <Box
              display="flex"
              alignItems="center"
              justifyContent="center"
              height="90%"
            >
              <Typography color="textSecondary">
                Gráfico circular (implementar con Recharts)
              </Typography>
            </Box>
          </Paper>
        </Grid>

        <Grid item xs={12}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Últimas Auditorías
            </Typography>
            <Box
              display="flex"
              alignItems="center"
              justifyContent="center"
              height={200}
            >
              <Typography color="textSecondary">
                Tabla de últimas auditorías (usar componente Table)
              </Typography>
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};