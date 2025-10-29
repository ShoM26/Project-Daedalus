// src/services/authService.js
const BASE_URL = 'http://localhost:5278/api';

class authService {
  // Call your C# login endpoint
  async login(username, password) {
    try {
      const response = await fetch(`${BASE_URL}/Users/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          username: username,
          password: password
        })
      });

      const data = await response.json();

      if (response.ok && data.success) {
        // Store JWT token and user info
        localStorage.setItem('token', data.token);
        localStorage.setItem('user', JSON.stringify({
          userId: data.userId,
          username: data.username,
          email: data.email
        }));
        
        return { success: true, user: data };
      } else {
        return { success: false, message: data.message };
      }
    } catch (error) {
      console.error('Login error:', error);
      return { success: false, message: 'Network error occurred' };
    }
  }

  async Signup(username, password, email){
    try{
      const response = await fetch(`${BASE_URL}/Users`,{
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          username: username,
          password: password,
          email: email,
        })
      });

      const data = await response.json();
      if(response.ok && data.success){
        localStorage.setItem('token', data.token);
        localStorage.setItem('user', JSON.stringify({
          userId: data.userId,
          username: data.username,
          email: data.email
        }));
        return { success: true, user: data};
      } else{
        return { success: false, message: data.message };
      }
    } catch(error){
      console.error('Signup error: ', error);
      return { success: false, message: 'Network error occured' };
    }
  }

  // Logout - clear stored data
  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  // Check if user is logged in
  isAuthenticated() {
    const token = localStorage.getItem('token');
    if (!token) return false;

    // Basic token expiry check (decode JWT to check exp claim)
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const now = Date.now() / 1000;
      
      if (payload.exp < now) {
        // Token expired, clean up
        this.logout();
        return false;
      }
      
      return true;
    } catch (error) {
      // Invalid token format
      this.logout();
      return false;
    }
  }

  // Get stored user info
  getCurrentUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }

  // Get JWT token for API calls
  getToken() {
    return localStorage.getItem('token');
  }

  // Get Authorization header for API calls
  getAuthHeader() {
    const token = this.getToken();
    return token ? { 'Authorization': `Bearer ${token}` } : {};
  }
}

export default new authService();