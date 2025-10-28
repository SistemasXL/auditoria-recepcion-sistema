export enum EstadoAuditoria {
  BORRADOR = 'Borrador',
  EN_PROCESO = 'En Proceso',
  FINALIZADA = 'Finalizada',
  CERRADA = 'Cerrada',
  CANCELADA = 'Cancelada'
}

export interface Auditoria {
  id: number;
  numeroAuditoria: string;
  proveedorId: number;
  proveedor: Proveedor;
  ordenCompraId: number;
  ordenCompra: string;
  fecha: string;
  hora: string;
  estado: EstadoAuditoria;
  usuarioCreadorId: number;
  usuarioCreador: string;
  observaciones?: string;
  totalProductos: number;
  totalCantidad: number;
  tieneIncidencias: boolean;
  fechaCreacion: string;
  fechaFinalizacion?: string;
  fechaCierre?: string;
}

export interface Proveedor {
  id: number;
  codigo: string;
  nombre: string;
  contacto?: string;
  email?: string;
  telefono?: string;
  activo: boolean;
}

export interface DetalleAuditoria extends Auditoria {
  productos: ProductoAuditoria[];
  evidencias: Evidencia[];
  incidencias: IncidenciaResumen[];
}

export interface ProductoAuditoria {
  id: number;
  auditoriaId: number;
  productoId: number;
  producto: ProductoInfo;
  cantidadEsperada: number;
  cantidadRecibida: number;
  cantidadDiferencia: number;
  fechaEscaneo?: string;
  observaciones?: string;
}

export interface ProductoInfo {
  id: number;
  sku: string;
  codigoBarras: string;
  nombre: string;
  descripcion?: string;
  unidadMedida: string;
  categoriaId?: number;
  categoria?: string;
  activo: boolean;
}

export interface Evidencia {
  id: number;
  auditoriaId: number;
  tipo: 'imagen' | 'video' | 'documento';
  nombreArchivo: string;
  rutaArchivo: string;
  urlPublica?: string;
  tamanioBytes: number;
  descripcion?: string;
  fechaSubida: string;
  usuarioSubidaId: number;
}

export interface IncidenciaResumen {
  id: number;
  tipo: string;
  severidad: string;
  descripcion: string;
  estado: string;
  fechaCreacion: string;
}

// DTOs para crear/actualizar
export interface CrearAuditoriaDto {
  proveedorId: number;
  ordenCompraId: number;
  fecha: string;
  hora: string;
  observaciones?: string;
}

export interface AgregarProductoDto {
  auditoriaId: number;
  productoId: number;
  cantidadEsperada: number;
  cantidadRecibida: number;
  observaciones?: string;
}

export interface ActualizarAuditoriaDto {
  observaciones?: string;
  estado?: EstadoAuditoria;
}