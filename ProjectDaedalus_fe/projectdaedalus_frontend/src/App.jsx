// src/App.jsx
import Dashboard from './pages/Dashboard';
import './App.css';

function App() {
  return (
    <div className="App">
      {/* For now, just show the Dashboard */}
      {/* Later we'll add React Router here for multiple pages */}
      <Dashboard />
    </div>
  );
}

export default App;