import { z } from 'zod';

// Mensajes de error personalizados
const errorMessages = {
  required: 'Este campo es requerido',
  email: 'Ingrese un email válido',
  min: (min: number) => `Debe tener al menos ${min} caracteres`,
  max: (max: number) => `Debe tener como máximo ${max} caracteres`,
  positive: 'Debe ser un número positivo',
  integer: 'Debe ser un número entero',
};

// Schemas de validación

// Login
export const loginSchema = z.object({
  username: z.string().min(3, errorMessages.min(3)),
  password: z.string().min(6, errorMessages.min(6)),
});

// Usuario
export const usuarioSchema = z.object({
  username: z.string().min(3, errorMessages.min(3)).max(50, errorMessages.max(50)),
  email: z.string().email(errorMessages.email),
  nombreCompleto: z.string().min(3, errorMessages.min(3)).max(100, errorMessages.max(100)),
  rolId: z.number().int().positive(errorMessages.positive),
  password: z.string().min(8, errorMessages.min(8)).optional(),
});

// Cambio de contraseña
export const changePasswordSchema = z.object({
  currentPassword: z.string().min(1, errorMessages.required),
  newPassword: z.string().min(8, errorMessages.min(8)),
  confirmPassword: z.string().min(1, errorMessages.required),
}).refine((data) => data.newPassword === data.confirmPassword, {
  message: 'Las contraseñas no coinciden',
  path: ['confirmPassword'],
});

// Auditoría
export const auditoriaSchema = z.object({
  proveedorId: z.number().int().positive(errorMessages.positive),
  ordenCompraId: z.number().int().positive(errorMessages.positive),
  fecha: z.string().min(1, errorMessages.required),
  hora: z.string().min(1, errorMessages.required),
  observaciones: z.string().max(1000, errorMessages.max(1000)).optional(),
});

// Producto en auditoría
export const productoAuditoriaSchema = z.object({
  productoId: z.number().int().positive(errorMessages.positive),
  cantidadEsperada: z.number().int().positive(errorMessages.positive),
  cantidadRecibida: z.number().int().min(0, 'No puede ser negativo'),
  observaciones: z.string().max(500, errorMessages.max(500)).optional(),
});

// Producto
export const productoSchema = z.object({
  sku: z.string().min(1, errorMessages.required).max(50, errorMessages.max(50)),
  codigoBarras: z.string().min(1, errorMessages.required).max(100, errorMessages.max(100)),
  nombre: z.string().min(3, errorMessages.min(3)).max(200, errorMessages.max(200)),
  descripcion: z.string().max(500, errorMessages.max(500)).optional(),
  unidadMedida: z.string().min(1, errorMessages.required).max(20, errorMessages.max(20)),
  categoriaId: z.number().int().positive().optional(),
});

// Incidencia
export const incidenciaSchema = z.object({
  auditoriaId: z.number().int().positive(errorMessages.positive),
  tipo: z.string().min(1, errorMessages.required),
  severidad: z.string().min(1, errorMessages.required),
  descripcion: z.string().min(10, errorMessages.min(10)).max(1000, errorMessages.max(1000)),
  productoAfectadoId: z.number().int().positive().optional(),
  cantidadAfectada: z.number().int().positive().optional(),
});

// Comentario de incidencia
export const comentarioIncidenciaSchema = z.object({
  comentario: z.string().min(5, errorMessages.min(5)).max(500, errorMessages.max(500)),
});

// Proveedor
export const proveedorSchema = z.object({
  codigo: z.string().min(1, errorMessages.required).max(20, errorMessages.max(20)),
  nombre: z.string().min(3, errorMessages.min(3)).max(200, errorMessages.max(200)),
  contacto: z.string().max(100, errorMessages.max(100)).optional(),
  email: z.string().email(errorMessages.email).optional().or(z.literal('')),
  telefono: z.string().max(20, errorMessages.max(20)).optional(),
});

// Validación de archivos
export const validateFile = (file: File, maxSizeBytes: number, allowedTypes: string[]): { valid: boolean; error?: string } => {
  // Validar tamaño
  if (file.size > maxSizeBytes) {
    return {
      valid: false,
      error: `El archivo excede el tamaño máximo permitido (${Math.round(maxSizeBytes / 1024 / 1024)}MB)`,
    };
  }

  // Validar tipo
  if (!allowedTypes.includes(file.type)) {
    return {
      valid: false,
      error: 'Tipo de archivo no permitido',
    };
  }

  return { valid: true };
};

// Validación de código de barras
export const validateBarcode = (barcode: string): boolean => {
  // Códigos de barras comunes: EAN-13, EAN-8, UPC-A, UPC-E, Code 39, Code 128
  const barcodeRegex = /^[0-9]{8,14}$|^[A-Z0-9\-\.\s\$\/\+%]+$/;
  return barcodeRegex.test(barcode);
};

// Validación de SKU
export const validateSKU = (sku: string): boolean => {
  // SKU alfanumérico con guiones permitidos
  const skuRegex = /^[A-Z0-9\-]+$/i;
  return skuRegex.test(sku);
};

// Validación de rango de fechas
export const validateDateRange = (startDate: string, endDate: string): boolean => {
  const start = new Date(startDate);
  const end = new Date(endDate);
  return start <= end;
};

// Tipos TypeScript a partir de los schemas
export type LoginFormData = z.infer<typeof loginSchema>;
export type UsuarioFormData = z.infer<typeof usuarioSchema>;
export type ChangePasswordFormData = z.infer<typeof changePasswordSchema>;
export type AuditoriaFormData = z.infer<typeof auditoriaSchema>;
export type ProductoAuditoriaFormData = z.infer<typeof productoAuditoriaSchema>;
export type ProductoFormData = z.infer<typeof productoSchema>;
export type IncidenciaFormData = z.infer<typeof incidenciaSchema>;
export type ComentarioIncidenciaFormData = z.infer<typeof comentarioIncidenciaSchema>;
export type ProveedorFormData = z.infer<typeof proveedorSchema>;