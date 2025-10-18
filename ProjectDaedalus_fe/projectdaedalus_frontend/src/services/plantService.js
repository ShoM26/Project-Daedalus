import apiService from './api';

export const plantService = {
  // Get all plants (original endpoint - not user-specific)
  getAllPlants: async () => {
    return await apiService.get('/Plants');
  },
  // Get user's plants with full details (plant info, device info)
  getUserPlants: async (userId) => {
    return await apiService.get(`/UserPlants/${userId}/plants`);
  },
  // Get latest sensor readings for a specific device
  getLatestReading: async (deviceId) => {
    const response =  await apiService.get(`/SensorReadings/device/${deviceId}/reading`);
    if (response.NoContent){
      return null;
    }
    return response;
  },
  deleteUserPlant: async () => {
    return await apiService.delete(`/UserPlants/${userPlantId}`);
  },
  getReadingsRange: async (deviceId, startDate, endDate) => {
    return await apiService.get(`/SensorReadings/device/${deviceId}/range?startDate=${startDate}&endDate=${endDate}`);
  },
  getDevicesByUser: async (userId) => {
    return await apiService.get(`/Devices/user/${userId}/devices`);
  },
  postUserPlant: async (userId, deviceId, plantId) => {
    try {
      const data = await apiService.post('/UserPlants', {
        userId: userId,
        deviceId: deviceId,
        plantId: plantId
      });
      return { success: true, data: data };
    } catch (error) {
      console.error('Error creating pairing:', error);
      return { success: false, message: error.message };
    }
  },

  putUserPlant: async (deviceId, userId, plantId) => {
    try {
      const data = await apiService.put(`/UserPlants/device/${deviceId}`, {
        userId: userId,
        deviceId: deviceId,
        plantId: plantId
      });
      return { success: true, data: data };
    } catch (error) {
      console.error('Error updating pairing:', error);
      return { success: false, message: error.message };
    }
  },
  deleteSensorReadingsByDevice: async (deviceId) => {
    try {
      await apiService.delete(`/SensorReadings/${deviceId}`);
      return { success: true };
    } catch (error) {
      console.error('Error updating pairing:', error);
      console.error('Error deleting sensor readings:', error);
      return { success: false, message: error.message };
    }
  }
      
};