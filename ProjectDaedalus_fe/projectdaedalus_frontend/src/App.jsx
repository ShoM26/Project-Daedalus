// src/App.jsx
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import Login from './pages/Login';
import Signup from './pages/Signup';
import LandingPage from './pages/LandingPage';
import authService from './services/authService';
import './App.css';

// Protected Route component - only shows content if user is logged in
function ProtectedRoute({ children }) {
  return authService.isAuthenticated() ? children : <Navigate to="/landingpage" replace />;
}

function AuthRoute({ children }) {
  return authService.isAuthenticated() ? 
    <Navigate to="/dashboard" replace /> : 
    children;
}

function App() {
  return (
    <div className="App">
      <Router>
        <Routes>
          <Route path="/" element={<AuthRoute><LandingPage/></AuthRoute>}/>
          <Route path="/signup" element={<AuthRoute><Signup/></AuthRoute>}/>
          {<Route path="/login" element={<AuthRoute><Login/></AuthRoute>} />}
          <Route 
            path="/dashboard" 
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            } 
          />
          {/* Catch all other routes - redirect to home */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Router>
    </div>
  );
}

export default App;