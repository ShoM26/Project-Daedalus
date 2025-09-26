import apiService from './api';

export const plantService = {
  // Get all plants from /api/Plants endpoint
  getAllPlants: async () => {
    return await apiService.get('/Plants');
  }
};