import apiService from './api';

export const notificationService = {
    getUnreadCount: async (userId) => {
        return await apiService.get(`/Notifications/${userId}/unread-count`);
    },
    getNotifications: async (userId) =>{
        return await apiService.get(`/Notifications/${userId}`);
    },
    markAsRead: async (notificationId, userId) => {
        return await apiService.patch(`/Notifications/${userId}/${notificationId}/read`); //TODO: Implement
    },
    markAllAsRead: async (userId) => {
        return await apiService.patch(`/Notifications/${userId}/mark-all-read`); //TODO: Implement
    }

};
