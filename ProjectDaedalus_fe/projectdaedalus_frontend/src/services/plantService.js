import apiService from './api';

export const plantService = {
  // Get all plants (original endpoint - not user-specific)
  getAllPlants: async () => {
    return await apiService.get('/Plants');
  },

  // Get user's plants with full details (plant info, device info)
  getUserPlants: async (userId) => {
    return await apiService.get(`/UserPlants/${userId}/plants`);
  }
/*
  // Get latest sensor readings for a specific device
  getLatestReading: async (deviceId) => {
    return await apiService.get(`/SensorReadings/device/${deviceId}/latest`);
  }*/
};