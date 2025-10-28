import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';

export type NotificationType = 'success' | 'error' | 'warning' | 'info';

export interface Notification {
  id: string;
  type: NotificationType;
  message: string;
  duration?: number;
}

interface UIState {
  // Sidebar
  sidebarOpen: boolean;
  sidebarCollapsed: boolean;

  // Modales
  modalOpen: boolean;
  modalContent: React.ReactNode | null;
  modalTitle: string;

  // Notificaciones
  notifications: Notification[];

  // Loading global
  globalLoading: boolean;

  // Theme
  theme: 'light' | 'dark';

  // Acciones - Sidebar
  toggleSidebar: () => void;
  setSidebarOpen: (open: boolean) => void;
  toggleSidebarCollapse: () => void;

  // Acciones - Modal
  openModal: (title: string, content: React.ReactNode) => void;
  closeModal: () => void;

  // Acciones - Notificaciones
  addNotification: (notification: Omit<Notification, 'id'>) => void;
  removeNotification: (id: string) => void;
  clearNotifications: () => void;

  // Acciones - Loading
  setGlobalLoading: (loading: boolean) => void;

  // Acciones - Theme
  toggleTheme: () => void;
  setTheme: (theme: 'light' | 'dark') => void;
}

export const useUIStore = create<UIState>()(
  devtools(
    persist(
      (set, get) => ({
        // Estado inicial
        sidebarOpen: true,
        sidebarCollapsed: false,
        modalOpen: false,
        modalContent: null,
        modalTitle: '',
        notifications: [],
        globalLoading: false,
        theme: 'light',

        // Sidebar
        toggleSidebar: () => {
          set((state) => ({ sidebarOpen: !state.sidebarOpen }));
        },
        setSidebarOpen: (open: boolean) => {
          set({ sidebarOpen: open });
        },
        toggleSidebarCollapse: () => {
          set((state) => ({ sidebarCollapsed: !state.sidebarCollapsed }));
        },

        // Modal
        openModal: (title: string, content: React.ReactNode) => {
          set({
            modalOpen: true,
            modalTitle: title,
            modalContent: content,
          });
        },
        closeModal: () => {
          set({
            modalOpen: false,
            modalTitle: '',
            modalContent: null,
          });
        },

        // Notificaciones
        addNotification: (notification: Omit<Notification, 'id'>) => {
          const id = `notification-${Date.now()}-${Math.random()}`;
          const newNotification: Notification = {
            ...notification,
            id,
            duration: notification.duration || 5000,
          };

          set((state) => ({
            notifications: [...state.notifications, newNotification],
          }));

          // Auto-remover después de la duración
          if (newNotification.duration) {
            setTimeout(() => {
              get().removeNotification(id);
            }, newNotification.duration);
          }
        },
        removeNotification: (id: string) => {
          set((state) => ({
            notifications: state.notifications.filter((n) => n.id !== id),
          }));
        },
        clearNotifications: () => {
          set({ notifications: [] });
        },

        // Loading global
        setGlobalLoading: (loading: boolean) => {
          set({ globalLoading: loading });
        },

        // Theme
        toggleTheme: () => {
          set((state) => ({
            theme: state.theme === 'light' ? 'dark' : 'light',
          }));
        },
        setTheme: (theme: 'light' | 'dark') => {
          set({ theme });
        },
      }),
      {
        name: 'ui-storage',
        partialize: (state) => ({
          sidebarCollapsed: state.sidebarCollapsed,
          theme: state.theme,
        }),
      }
    ),
    { name: 'UIStore' }
  )
);