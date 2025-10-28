import { apiService } from './api.service';
import { ENDPOINTS } from '@config/api.config';
import { ProductoInfo } from '@types/auditoria.types';
import { PaginatedResponse, PaginationParams, FilterParams } from '@types/api.types';

class ProductosService {
  /**
   * Obtener lista de productos con paginación
   */
  async getProductos(
    pagination: PaginationParams,
    filters?: FilterParams
  ): Promise<PaginatedResponse<ProductoInfo>> {
    const params = {
      ...pagination,
      ...filters,
    };

    const response = await apiService.get<PaginatedResponse<ProductoInfo>>(
      ENDPOINTS.PRODUCTOS.BASE,
      { params }
    );

    return response.data;
  }

  /**
   * Obtener producto por ID
   */
  async getProductoById(id: number): Promise<ProductoInfo> {
    const response = await apiService.get<ProductoInfo>(
      ENDPOINTS.PRODUCTOS.BY_ID(id)
    );
    return response.data;
  }

  /**
   * Buscar producto por SKU
   */
  async getProductoBySku(sku: string): Promise<ProductoInfo> {
    const response = await apiService.get<ProductoInfo>(
      ENDPOINTS.PRODUCTOS.BY_SKU(sku)
    );
    return response.data;
  }

  /**
   * Buscar producto por código de barras
   */
  async getProductoByBarcode(barcode: string): Promise<ProductoInfo> {
    const response = await apiService.get<ProductoInfo>(
      ENDPOINTS.PRODUCTOS.BY_BARCODE(barcode)
    );
    return response.data;
  }

  /**
   * Buscar productos (texto libre)
   */
  async searchProductos(query: string): Promise<ProductoInfo[]> {
    const response = await apiService.get<ProductoInfo[]>(
      ENDPOINTS.PRODUCTOS.SEARCH,
      { params: { q: query } }
    );
    return response.data;
  }

  /**
   * Crear nuevo producto
   */
  async createProducto(data: Partial<ProductoInfo>): Promise<ProductoInfo> {
    const response = await apiService.post<ProductoInfo>(
      ENDPOINTS.PRODUCTOS.BASE,
      data
    );
    return response.data;
  }

  /**
   * Actualizar producto
   */
  async updateProducto(id: number, data: Partial<ProductoInfo>): Promise<ProductoInfo> {
    const response = await apiService.put<ProductoInfo>(
      ENDPOINTS.PRODUCTOS.BY_ID(id),
      data
    );
    return response.data;
  }

  /**
   * Eliminar producto
   */
  async deleteProducto(id: number): Promise<void> {
    await apiService.delete(ENDPOINTS.PRODUCTOS.BY_ID(id));
  }

  /**
   * Activar/desactivar producto
   */
  async toggleProductoActivo(id: number, activo: boolean): Promise<ProductoInfo> {
    const response = await apiService.patch<ProductoInfo>(
      ENDPOINTS.PRODUCTOS.BY_ID(id),
      { activo }
    );
    return response.data;
  }
}

export const productosService = new ProductosService();