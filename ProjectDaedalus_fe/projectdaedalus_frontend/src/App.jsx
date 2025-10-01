// src/App.jsx
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import Login from './pages/Login';
import authService from './services/authService';
import './App.css';

// Protected Route component - only shows content if user is logged in
function ProtectedRoute({ children }) {
  return authService.isAuthenticated() ? children : <Navigate to="/login" />;
}

function App() {
  return (
    <div className="App">
      <Router>
        <Routes>
          {/* Login page - accessible to everyone */}
          <Route path="/login" element={<Login />} />
          
          {/* Dashboard - protected route, requires authentication */}
          <Route 
            path="/dashboard" 
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            } 
          />
          
          {/* Default route - redirect based on auth status */}
          <Route 
            path="/" 
            element={
              authService.isAuthenticated() ? 
                <Navigate to="/dashboard" /> : 
                <Navigate to="/login" />
            } 
          />
          
          {/* Catch all other routes - redirect to home */}
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </Router>
    </div>
  );
}

export default App;