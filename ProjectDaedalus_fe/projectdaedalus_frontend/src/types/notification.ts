export interface Notification {
  notificationId: string;
  message: string;
  notificationType: string;
  isRead: boolean;
  createdAt: string;
  userPlantId: string;
  userPlantName: string;
}

export interface NotificationState {
  notifications: Notification[];
  unreadCount: number;
  loading: boolean;
  error: string | null;
}