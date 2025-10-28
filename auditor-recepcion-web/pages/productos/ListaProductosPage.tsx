import React, { useEffect, useState } from 'react';
import {
  Box,
  Typography,
  TextField,
  InputAdornment,
  Chip,
} from '@mui/material';
import {
  Search as SearchIcon,
} from '@mui/icons-material';
import { Table, Column } from '@components/common/Table';
import { productosService } from '@services/productos.service';
import { ProductoInfo } from '@types/auditoria.types';
import toast from 'react-hot-toast';

export const ListaProductosPage: React.FC = () => {
  const [productos, setProductos] = useState<ProductoInfo[]>([]);
  const [loading, setLoading] = useState(false);
  const [totalItems, setTotalItems] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadProductos();
  }, [currentPage, pageSize]);

  const loadProductos = async () => {
    setLoading(true);
    try {
      const filters = searchTerm ? { search: searchTerm } : undefined;
      const response = await productosService.getProductos(
        { page: currentPage, pageSize },
        filters
      );
      setProductos(response.items);
      setTotalItems(response.totalItems);
    } catch (error) {
      console.error('Error al cargar productos:', error);
      toast.error('Error al cargar productos');
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    setCurrentPage(1);
    loadProductos();
  };

  const handlePageChange = (newPage: number) => {
    setCurrentPage(newPage + 1);
  };

  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize);
    setCurrentPage(1);
  };

  const columns: Column<ProductoInfo>[] = [
    {
      id: 'sku',
      label: 'SKU',
      width: 150,
      render: (row) => (
        <Typography variant="body2" fontWeight="medium">
          {row.sku}
        </Typography>
      ),
    },
    {
      id: 'nombre',
      label: 'Producto',
      render: (row) => (
        <Box>
          <Typography variant="body2">{row.nombre}</Typography>
          {row.descripcion && (
            <Typography variant="caption" color="textSecondary">
              {row.descripcion}
            </Typography>
          )}
        </Box>
      ),
    },
    {
      id: 'codigoBarras',
      label: 'Código de Barras',
      width: 180,
    },
    {
      id: 'categoria',
      label: 'Categoría',
      width: 150,
      render: (row) => row.categoria || '-',
    },
    {
      id: 'unidadMedida',
      label: 'Unidad',
      width: 100,
      align: 'center',
    },
    {
      id: 'activo',
      label: 'Estado',
      width: 120,
      align: 'center',
      render: (row) => (
        <Chip
          label={row.activo ? 'Activo' : 'Inactivo'}
          color={row.activo ? 'success' : 'default'}
          size="small"
        />
      ),
    },
  ];

  return (
    <Box>
      {/* Header */}
      <Box mb={3}>
        <Typography variant="h4" fontWeight="bold" gutterBottom>
          Productos
        </Typography>
        <Typography variant="body2" color="textSecondary">
          Catálogo de productos del sistema
        </Typography>
      </Box>

      {/* Búsqueda */}
      <Box mb={3}>
        <TextField
          placeholder="Buscar por SKU, nombre o código de barras..."
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
        data={productos}
        loading={loading}
        pagination
        page={currentPage - 1}
        pageSize={pageSize}
        totalItems={totalItems}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        emptyMessage="No se encontraron productos"
      />
    </Box>
  );
};