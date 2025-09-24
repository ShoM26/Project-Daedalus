// src/pages/Dashboard.jsx
import { useState } from 'react';
import PlantCard from '../components/PlantCard';
import { mockPlantPairings } from '../data/mockData';

function Dashboard() {
  // STATE - data that can change and triggers re-renders
  const [plants, setPlants] = useState(mockPlantPairings);
  const [selectedFilter, setSelectedFilter] = useState('all');

  // DERIVED STATE - calculated from existing state
  const healthyPlants = plants.filter(p => p.status === 'healthy').length;
  const needsAttention = plants.filter(p => p.status !== 'healthy').length;

  // EVENT HANDLER - function that responds to user actions
  const handleFilterChange = (filter) => {
    setSelectedFilter(filter);
  };

  // FILTERED DATA - show only plants matching current filter
  const filteredPlants = plants.filter(plant => {
    if (selectedFilter === 'all') return true;
    if (selectedFilter === 'needs-attention') return plant.status !== 'healthy';
    return plant.status === selectedFilter;
  });

  return (
    <div className="dashboard">
      {/* Dashboard Header */}
      <header className="dashboard-header">
        <h1>🌱 Plant Monitor Dashboard</h1>
        <div className="summary-stats">
          <div className="stat">
            <span className="stat-number">{plants.length}</span>
            <span className="stat-label">Total Plants</span>
          </div>
          <div className="stat healthy">
            <span className="stat-number">{healthyPlants}</span>
            <span className="stat-label">Healthy</span>
          </div>
          <div className="stat attention">
            <span className="stat-number">{needsAttention}</span>
            <span className="stat-label">Need Attention</span>
          </div>
        </div>
      </header>

      {/* Filter Controls */}
      <div className="filter-controls">
        <h3>Filter Plants:</h3>
        <div className="filter-buttons">
          {/* Each button calls handleFilterChange with different values */}
          <button 
            className={selectedFilter === 'all' ? 'active' : ''}
            onClick={() => handleFilterChange('all')}
          >
            All Plants ({plants.length})
          </button>
          <button 
            className={selectedFilter === 'healthy' ? 'active' : ''}
            onClick={() => handleFilterChange('healthy')}
          >
            Healthy ({healthyPlants})
          </button>
          <button 
            className={selectedFilter === 'needs_water' ? 'active' : ''}
            onClick={() => handleFilterChange('needs_water')}
          >
            Needs Water
          </button>
          <button 
            className={selectedFilter === 'needs-attention' ? 'active' : ''}
            onClick={() => handleFilterChange('needs-attention')}
          >
            Needs Attention ({needsAttention})
          </button>
        </div>
      </div>

      {/* Plants Grid */}
      <div className="plants-grid">
        {/* MAPPING - create a PlantCard for each plant */}
        {filteredPlants.map(pairing => (
          <PlantCard 
            key={pairing.id}  // Required for React lists
            pairing={pairing} // Pass entire pairing object as prop
          />
        ))}
        
        {/* CONDITIONAL RENDERING - show message if no plants match filter */}
        {filteredPlants.length === 0 && (
          <div className="no-plants">
            <p>No plants match the current filter.</p>
          </div>
        )}
      </div>
    </div>
  );
}

export default Dashboard;