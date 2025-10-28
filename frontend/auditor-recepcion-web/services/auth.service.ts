import { apiService } from './api.service';
import { ENDPOINTS } from '@config/api.config';
import { STORAGE_KEYS } from '@config/constants';
import { 
  Usuario, 
  LoginCredentials, 
  AuthResponse 
} from '@types/usuario.types';
import { ApiResponse } from '@types/api.types';

class AuthService {
  /**
   * Login de usuario
   */
  async login(credentials: LoginCredentials): Promise<AuthResponse> {
    const response = await apiService.post<AuthResponse>(
      ENDPOINTS.AUTH.LOGIN,
      credentials
    );

    if (response.success && response.data) {
      this.saveAuthData(response.data);
    }

    return response.data;
  }

  /**
   * Logout de usuario
   */
  async logout(): Promise<void> {
    try {
      await apiService.post(ENDPOINTS.AUTH.LOGOUT);
    } finally {
      this.clearAuthData();
    }
  }

  /**
   * Obtener información del usuario actual
   */
  async getCurrentUser(): Promise<Usuario> {
    const response = await apiService.get<Usuario>(ENDPOINTS.AUTH.ME);
    return response.data;
  }

  /**
   * Cambiar contraseña
   */
  async changePassword(currentPassword: string, newPassword: string): Promise<void> {
    await apiService.post(ENDPOINTS.AUTH.CHANGE_PASSWORD, {
      currentPassword,
      newPassword,
    });
  }

  /**
   * Verificar si el usuario está autenticado
   */
  isAuthenticated(): boolean {
    const token = localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);
    return !!token;
  }

  /**
   * Obtener token actual
   */
  getToken(): string | null {
    return localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);
  }

  /**
   * Obtener datos del usuario del localStorage
   */
  getUserData(): Usuario | null {
    const userData = localStorage.getItem(STORAGE_KEYS.USER_DATA);
    return userData ? JSON.parse(userData) : null;
  }

  /**
   * Guardar datos de autenticación
   */
  private saveAuthData(authData: AuthResponse): void {
    localStorage.setItem(STORAGE_KEYS.AUTH_TOKEN, authData.token);
    localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, authData.refreshToken);
    localStorage.setItem(STORAGE_KEYS.USER_DATA, JSON.stringify(authData.usuario));
  }

  /**
   * Limpiar datos de autenticación
   */
  private clearAuthData(): void {
    localStorage.removeItem(STORAGE_KEYS.AUTH_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.USER_DATA);
  }

  /**
   * Verificar si el token ha expirado
   */
  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp * 1000; // Convertir a milisegundos
      return Date.now() >= expiry;
    } catch (error) {
      return true;
    }
  }
}

export const authService = new AuthService();