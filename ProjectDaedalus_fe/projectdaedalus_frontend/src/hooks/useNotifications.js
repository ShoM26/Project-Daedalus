// hooks/useNotifications.js
import { useState, useCallback, useEffect } from 'react';
import { notificationService } from '../services/notificationService';
import authService from '../services/authService';

export function useNotifications() {
  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchNotifications = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const currentUser = authService.getCurrentUser();
      const apiNotifications = await notificationService.getNotifications(currentUser.userId);

      if (!apiNotifications || !Array.isArray(apiNotifications)) {
        setNotifications([]);
        return;
      }

      const transformedNotifications = apiNotifications.map(notification => ({
        notificationId: notification.notificationId,
        message: notification.message,
        notificationType: notification.notificationType,
        isRead: notification.isRead,
        createdAt: notification.createdAt,
        userPlantId: notification.userPlantId,
        userPlantName: notification.userPlantName
      }));
      
      setNotifications(transformedNotifications);
    } catch (err) {
      console.error('Failed to fetch notifications:', err);
      setError('Failed to load notifications. Please try again.');
    } finally {
      setLoading(false);
    }
  }, []);

  const fetchUnreadCount = useCallback(async () => {
    try {
      const currentUser = authService.getCurrentUser();
      const data = await notificationService.getUnreadCount(currentUser.userId);
      setUnreadCount(data.unreadCount || 0);
    } catch (error) {
      console.error('Error fetching unread count:', error);
    }
  }, []);

  const markAsRead = useCallback(async (notificationId) => {
    const currentUser = authService.getCurrentUser();
    try {
      await notificationService.markAsRead(notificationId, currentUser.userId);
      
      // Optimistic update
      setNotifications(prev => 
        prev.map(n => n.notificationId === notificationId ? { ...n, isRead: true } : n)
      );
      setUnreadCount(prev => Math.max(0, prev - 1));
    } catch (error) {
      console.error('Error marking as read:', error);
      // Rollback on error
      await fetchNotifications();
      await fetchUnreadCount();
    }
  }, [fetchNotifications, fetchUnreadCount]);

  const markAllAsRead = useCallback(async () => {
    const currentUser = authService.getCurrentUser();
    try {
      await notificationService.markAllAsRead(currentUser.userId);
      
      // Optimistic update
      setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
      setUnreadCount(0);
    } catch (error) {
      console.error('Error marking all as read:', error);
      // Rollback on error
      await fetchNotifications();
      await fetchUnreadCount();
    }
  }, [fetchNotifications, fetchUnreadCount]);

  // Auto-refresh unread count
  useEffect(() => {
    fetchUnreadCount();
    
    const intervalId = setInterval(() => {
      if (document.visibilityState === 'visible') {
        fetchUnreadCount();
      }
    }, 2 * 60 * 1000);

    return () => clearInterval(intervalId);
  }, [fetchUnreadCount]);

  return {
    notifications,
    unreadCount,
    loading,
    error,
    fetchNotifications,
    fetchUnreadCount,
    markAsRead,
    markAllAsRead
  };
}