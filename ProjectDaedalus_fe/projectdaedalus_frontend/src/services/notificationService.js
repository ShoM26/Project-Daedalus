import apiService from './api';

export const notificationService = {
    getUnreadCount: async () => {
        return await apiService.get('');
    },
    getNotifications: async () =>{
        return await apiService.get('');
    },
    markAsRead: async () => {
        return await apiService.patch(''); //TODO: Implement
    },
    markAllAsRead: async () => {
        return await apiService.patch(''); //TODO: Implement
    }

};
