import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';
import { authService } from '@services/auth.service';
import { Usuario, LoginCredentials } from '@types/usuario.types';
import { STORAGE_KEYS } from '@config/constants';

interface AuthState {
  // Estado
  usuario: Usuario | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;

  // Acciones
  login: (credentials: LoginCredentials) => Promise<void>;
  logout: () => Promise<void>;
  loadUser: () => Promise<void>;
  clearError: () => void;
  checkAuth: () => boolean;
}

export const useAuthStore = create<AuthState>()(
  devtools(
    persist(
      (set, get) => ({
        // Estado inicial
        usuario: null,
        isAuthenticated: false,
        isLoading: false,
        error: null,

        // Login
        login: async (credentials: LoginCredentials) => {
          set({ isLoading: true, error: null });
          try {
            const authData = await authService.login(credentials);
            set({
              usuario: authData.usuario,
              isAuthenticated: true,
              isLoading: false,
              error: null,
            });
          } catch (error: any) {
            set({
              error: error.message || 'Error al iniciar sesión',
              isLoading: false,
              isAuthenticated: false,
            });
            throw error;
          }
        },

        // Logout
        logout: async () => {
          set({ isLoading: true });
          try {
            await authService.logout();
          } catch (error) {
            console.error('Error during logout:', error);
          } finally {
            set({
              usuario: null,
              isAuthenticated: false,
              isLoading: false,
              error: null,
            });
          }
        },

        // Cargar usuario actual
        loadUser: async () => {
          if (!authService.isAuthenticated()) {
            set({ isAuthenticated: false, usuario: null });
            return;
          }

          set({ isLoading: true });
          try {
            const usuario = await authService.getCurrentUser();
            set({
              usuario,
              isAuthenticated: true,
              isLoading: false,
              error: null,
            });
          } catch (error: any) {
            set({
              usuario: null,
              isAuthenticated: false,
              isLoading: false,
              error: error.message,
            });
          }
        },

        // Limpiar error
        clearError: () => set({ error: null }),

        // Verificar autenticación
        checkAuth: () => {
          const isAuth = authService.isAuthenticated();
          if (!isAuth) {
            set({ isAuthenticated: false, usuario: null });
          }
          return isAuth;
        },
      }),
      {
        name: STORAGE_KEYS.USER_DATA,
        partialize: (state) => ({
          usuario: state.usuario,
          isAuthenticated: state.isAuthenticated,
        }),
      }
    ),
    { name: 'AuthStore' }
  )
);