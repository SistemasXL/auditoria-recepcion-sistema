import { apiService } from './api.service';
import { ProductoInfo } from '@types/auditoria.types';
import { ApiResponse, PaginatedResponse, PaginationParams } from '@types/api.types';

const getProductos = async (
  pagination: PaginationParams,
  filters?: any
): Promise<PaginatedResponse<ProductoInfo>> => {
  const response = await apiService.get<PaginatedResponse<ProductoInfo>>('/productos', {
    params: {
      page: pagination.page,
      pageSize: pagination.pageSize,
      ...filters,
    },
  });
  return response.data;
};

const getProductoById = async (id: number): Promise<ProductoInfo> => {
  const response = await apiService.get<ApiResponse<ProductoInfo>>(`/productos/${id}`);
  return response.data.data;
};

const buscarPorCodigo = async (codigo: string): Promise<ProductoInfo> => {
  const response = await apiService.get<ApiResponse<ProductoInfo>>('/productos/buscar', {
    params: { codigo },
  });
  return response.data.data;
};

export const productosService = {
  getProductos,
  getProductoById,
  buscarPorCodigo,
};