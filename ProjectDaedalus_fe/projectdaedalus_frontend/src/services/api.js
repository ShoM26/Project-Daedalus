// Base URL for your C# API
const BASE_URL = 'http://localhost:5278/api';

// Basic API service class
class ApiService {
  async get(endpoint) {
    try {
      const response = await fetch(`${BASE_URL}${endpoint}`);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const data = await response.json();
      return data;
    } catch (error) {
      console.error('API Error:', error);
      throw error;
    }
  }

  async delete(endpoint) {
    const token = localStorage.getItem('token');
    
    const response = await fetch(`${this.baseURL}${endpoint}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    // DELETE requests often return 204 No Content, so check before parsing
    if (response.status === 204) {
      return null;
    }

    return await response.json();
  }
}

export default new ApiService();