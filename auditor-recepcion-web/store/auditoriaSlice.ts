import { create } from 'zustand';
import { devtools } from 'zustand/middleware';
import { auditoriasService } from '@services/auditorias.service';
import { 
  Auditoria, 
  DetalleAuditoria, 
  ProductoAuditoria,
  Evidencia 
} from '@types/auditoria.types';

interface AuditoriaState {
  // Estado
  auditorias: Auditoria[];
  auditoriaActual: DetalleAuditoria | null;
  productosAuditoria: ProductoAuditoria[];
  evidencias: Evidencia[];
  totalItems: number;
  currentPage: number;
  isLoading: boolean;
  error: string | null;

  // Acciones
  fetchAuditorias: (page: number, pageSize: number, filters?: any) => Promise<void>;
  fetchAuditoriaById: (id: number) => Promise<void>;
  createAuditoria: (data: any) => Promise<Auditoria>;
  updateAuditoria: (id: number, data: any) => Promise<void>;
  deleteAuditoria: (id: number) => Promise<void>;
  agregarProducto: (data: any) => Promise<void>;
  eliminarProducto: (auditoriaId: number, productoId: number) => Promise<void>;
  subirEvidencia: (auditoriaId: number, file: File, descripcion?: string) => Promise<void>;
  finalizarAuditoria: (id: number) => Promise<void>;
  cerrarAuditoria: (id: number) => Promise<void>;
  clearAuditoriaActual: () => void;
  clearError: () => void;
}

export const useAuditoriaStore = create<AuditoriaState>()(
  devtools(
    (set, get) => ({
      // Estado inicial
      auditorias: [],
      auditoriaActual: null,
      productosAuditoria: [],
      evidencias: [],
      totalItems: 0,
      currentPage: 1,
      isLoading: false,
      error: null,

      // Fetch auditorías con paginación
      fetchAuditorias: async (page: number, pageSize: number, filters?: any) => {
        set({ isLoading: true, error: null });
        try {
          const response = await auditoriasService.getAuditorias(
            { page, pageSize },
            filters
          );
          set({
            auditorias: response.items,
            totalItems: response.totalItems,
            currentPage: response.currentPage,
            isLoading: false,
          });
        } catch (error: any) {
          set({
            error: error.message || 'Error al cargar auditorías',
            isLoading: false,
          });
        }
      },

      // Fetch auditoría por ID
      fetchAuditoriaById: async (id: number) => {
        set({ isLoading: true, error: null });
        try {
          const auditoria = await auditoriasService.getAuditoriaById(id);
          set({
            auditoriaActual: auditoria,
            productosAuditoria: auditoria.productos || [],
            evidencias: auditoria.evidencias || [],
            isLoading: false,
          });
        } catch (error: any) {
          set({
            error: error.message || 'Error al cargar auditoría',
            isLoading: false,
          });
        }
      },

      // Crear auditoría
      createAuditoria: async (data: any) => {
        set({ isLoading: true, error: null });
        try {
          const nuevaAuditoria = await auditoriasService.createAuditoria(data);
          set((state) => ({
            auditorias: [nuevaAuditoria, ...state.auditorias],
            isLoading: false,
          }));
          return nuevaAuditoria;
        } catch (error: any) {
          set({
            error: error.message || 'Error al crear auditoría',
            isLoading: false,
          });
          throw error;
        }
      },

      // Actualizar auditoría
      updateAuditoria: async (id: number, data: any) => {
        set({ isLoading: true, error: null });
        try {
          const auditoriaActualizada = await auditoriasService.updateAuditoria(id, data);
          set((state) => ({
            auditorias: state.auditorias.map((a) =>
              a.id === id ? auditoriaActualizada : a
            ),
            auditoriaActual: state.auditoriaActual?.id === id
              ? { ...state.auditoriaActual, ...auditoriaActualizada }
              : state.auditoriaActual,
            isLoading: false,
          }));
        } catch (error: any) {
          set({
            error: error.message || 'Error al actualizar auditoría',
            isLoading: false,
          });
          throw error;
        }
      },

      // Eliminar auditoría
      deleteAuditoria: async (id: number) => {
        set({ isLoading: true, error: null });
        try {
          await auditoriasService.deleteAuditoria(id);
          set((state) => ({
            auditorias: state.auditorias.filter((a) => a.id !== id),
            isLoading: false,
          }));
        } catch (error: any) {
          set({
            error: error.message || 'Error al eliminar auditoría',
            isLoading: false,
          });
          throw error;
        }
      },

      // Agregar producto
      agregarProducto: async (data: any) => {
        set({ isLoading: true, error: null });
        try {
          const producto = await auditoriasService.agregarProducto(data);
          set((state) => ({
            productosAuditoria: [...state.productosAuditoria, producto],
            isLoading: false,
          }));
        } catch (error: any) {
          set({
            error: error.message || 'Error al agregar producto',
            isLoading: false,
          });
          throw error;
        }
      },

      // Eliminar producto
      eliminarProducto: async (auditoriaId: number, productoId: number) => {
        set({ isLoading: true, error: null });
        try {
          await auditoriasService.eliminarProducto(auditoriaId, productoId);
          set((state) => ({
            productosAuditoria: state.productosAuditoria.filter(
              (p) => p.id !== productoId
            ),
            isLoading: false,
          }));
        } catch (error: any) {
          set({
            error: error.message || 'Error al eliminar producto',
            isLoading: false,
          });
          throw error;
        }
      },

      // Subir evidencia
      subirEvidencia: async (auditoriaId: number, file: File, descripcion?: string) => {
        set({ isLoading: true, error: null });
        try {
          const evidencia = await auditoriasService.subirEvidencia(
            auditoriaId,
            file,
            descripcion
          );
          set((state) => ({
            evidencias: [...state.evidencias, evidencia],
            isLoading: false,
          }));
        } catch (error: any) {
          set({
            error: error.message || 'Error al subir evidencia',
            isLoading: false,
          });
          throw error;
        }
      },

      // Finalizar auditoría
      finalizarAuditoria: async (id: number) => {
        set({ isLoading: true, error: null });
        try {
          const auditoria = await auditoriasService.finalizarAuditoria(id);
          set((state) => ({
            auditorias: state.auditorias.map((a) => (a.id === id ? auditoria : a)),
            auditoriaActual: state.auditoriaActual?.id === id
              ? { ...state.auditoriaActual, ...auditoria }
              : state.auditoriaActual,
            isLoading: false,
          }));
        } catch (error: any) {
          set({
            error: error.message || 'Error al finalizar auditoría',
            isLoading: false,
          });
          throw error;
        }
      },

      // Cerrar auditoría
      cerrarAuditoria: async (id: number) => {
        set({ isLoading: true, error: null });
        try {
          const auditoria = await auditoriasService.cerrarAuditoria(id);
          set((state) => ({
            auditorias: state.auditorias.map((a) => (a.id === id ? auditoria : a)),
            auditoriaActual: state.auditoriaActual?.id === id
              ? { ...state.auditoriaActual, ...auditoria }
              : state.auditoriaActual,
            isLoading: false,
          }));
        } catch (error: any) {
          set({
            error: error.message || 'Error al cerrar auditoría',
            isLoading: false,
          });
          throw error;
        }
      },

      // Limpiar auditoría actual
      clearAuditoriaActual: () => {
        set({
          auditoriaActual: null,
          productosAuditoria: [],
          evidencias: [],
        });
      },

      // Limpiar error
      clearError: () => set({ error: null }),
    }),
    { name: 'AuditoriaStore' }
  )
);