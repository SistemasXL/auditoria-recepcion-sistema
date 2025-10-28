export enum TipoIncidencia {
  FALTANTE = 'Faltante',
  SOBRANTE = 'Sobrante',
  PRODUCTO_DANADO = 'Producto Dañado',
  PRODUCTO_INCORRECTO = 'Producto Incorrecto',
  CALIDAD_DEFICIENTE = 'Calidad Deficiente',
  EMPAQUE_DANADO = 'Empaque Dañado',
  DOCUMENTACION_INCORRECTA = 'Documentación Incorrecta',
  OTRO = 'Otro'
}

export enum SeveridadIncidencia {
  BAJA = 'Baja',
  MEDIA = 'Media',
  ALTA = 'Alta',
  CRITICA = 'Crítica'
}

export enum EstadoIncidencia {
  ABIERTA = 'Abierta',
  EN_REVISION = 'En Revisión',
  RESUELTA = 'Resuelta',
  CERRADA = 'Cerrada',
  RECHAZADA = 'Rechazada'
}

export interface Incidencia {
  id: number;
  numeroIncidencia: string;
  auditoriaId: number;
  auditoria: {
    numeroAuditoria: string;
    proveedor: string;
    fecha: string;
  };
  tipo: TipoIncidencia;
  severidad: SeveridadIncidencia;
  estado: EstadoIncidencia;
  descripcion: string;
  productoAfectadoId?: number;
  productoAfectado?: string;
  cantidadAfectada?: number;
  accionCorrectiva?: string;
  responsableId?: number;
  responsable?: string;
  fechaCreacion: string;
  fechaResolucion?: string;
  fechaCierre?: string;
  usuarioCreadorId: number;
  usuarioCreador: string;
  comentarios: ComentarioIncidencia[];
}

export interface ComentarioIncidencia {
  id: number;
  incidenciaId: number;
  usuarioId: number;
  usuario: string;
  comentario: string;
  fecha: string;
}

export interface CrearIncidenciaDto {
  auditoriaId: number;
  tipo: TipoIncidencia;
  severidad: SeveridadIncidencia;
  descripcion: string;
  productoAfectadoId?: number;
  cantidadAfectada?: number;
}

export interface ActualizarIncidenciaDto {
  estado?: EstadoIncidencia;
  accionCorrectiva?: string;
  responsableId?: number;
}

export interface EstadisticasIncidencias {
  totalIncidencias: number;
  incidenciasAbiertas: number;
  incidenciasPorTipo: Record<TipoIncidencia, number>;
  incidenciasPorSeveridad: Record<SeveridadIncidencia, number>;
  promedioResolucionDias: number;
  proveedoresConMasIncidencias: {
    proveedorId: number;
    proveedor: string;
    cantidad: number;
  }[];
}