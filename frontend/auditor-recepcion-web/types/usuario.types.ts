export enum RolUsuario {
  OPERADOR = 'Operador',
  JEFE_LOGISTICA = 'Jefe de Logística',
  COMPRADOR = 'Comprador',
  ADMINISTRADOR = 'Administrador'
}

export interface Usuario {
  id: number;
  username: string;
  email: string;
  nombreCompleto: string;
  rolId: number;
  rol: RolUsuario;
  activo: boolean;
  fechaCreacion: string;
  ultimoAcceso?: string;
}

export interface LoginCredentials {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  usuario: Usuario;
  expiresIn: number;
}

export interface PermisoUsuario {
  id: number;
  nombre: string;
  descripcion: string;
  modulo: string;
}

export interface RolDetalle {
  id: number;
  nombre: RolUsuario;
  descripcion: string;
  permisos: PermisoUsuario[];
}

// Para auditoría de acciones
export interface AccionUsuario {
  id: number;
  usuarioId: number;
  usuario: string;
  accion: string;
  modulo: string;
  descripcion: string;
  ip: string;
  fecha: string;
  detalles?: Record<string, any>;
}