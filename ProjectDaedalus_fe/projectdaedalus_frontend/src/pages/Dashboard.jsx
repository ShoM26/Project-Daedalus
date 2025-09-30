import { useState, useEffect } from 'react';
import PlantCard from '../components/PlantCard';
import { plantService } from '../services/plantService';
import  AuthService  from '../services/authService';

function Dashboard() {
  // STATE - data that can change and triggers re-renders
  const [plants, setPlants] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedFilter, setSelectedFilter] = useState('all');

  // FETCH DATA FROM API when component loads
  useEffect(() => {
    const fetchPlants = async () => {
      try {
        setLoading(true);
        const currentUser = AuthService.getCurrentUser();
        const apiPlants = await plantService.getUserPlants(currentUser.userId);
        
        // Transform API data to match component expectations
        const transformedPlants = apiPlants.map(userPlant => ({
          id: userPlant.plantId,
          plant: {
            id: userPlant.plant.plantId,
            scientificName: userPlant.plant.scientificName,
            familiarName: userPlant.plant.familiarName,
            idealMoistureMin: userPlant.plant.moistureLowRange,
            idealMoistureMax: userPlant.plant.moistureHighRange,
            funFact: userPlant.plant.funFact || "This plant is part of your monitoring system!"
          },
          // Mock device and sensor data since API doesn't provide it yet
          device: {
            id: `DEVICE_${userPlant.plant.plantId}`,
            name: `${userPlant.plant.familiarName} Sensor`,
            connectionType: "Bluetooth",
            lastSeen: new Date().toISOString()
          },
          currentReading: {
            moistureLevel: Math.floor(Math.random() * 100), // Random for now
            timestamp: new Date().toISOString(),
            batteryLevel: Math.floor(Math.random() * 30) + 70 // 70-100%
          },
          status: "healthy" // Default status for now
        }));
        
        setPlants(transformedPlants);
        setError(null);
      } catch (err) {
        console.error('Failed to fetch plants:', err);
        setError('Failed to load plants. Please try again.');
      } finally {
        setLoading(false);
      }
    };

    fetchPlants();
  }, []);

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
        {loading ? (
          <p>Loading plants...</p>
        ) : error ? (
          <p style={{ color: 'red' }}>Error: {error}</p>
        ) : (
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
        )}
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
        {loading ? (
          <div className="loading">Loading plants...</div>
        ) : error ? (
          <div className="error">
            <p>Failed to load plants.</p>
            <button onClick={() => window.location.reload()}>Retry</button>
          </div>
        ) : (
          <>
            {/* MAPPING - create a PlantCard for each plant */}
            {filteredPlants.map(pairing => (
              <PlantCard 
                key={pairing.id}  // Required for React lists
                pairing={pairing} // Pass entire pairing object as prop
              />
            ))}
            
            {/* CONDITIONAL RENDERING - show message if no plants match filter */}
            {filteredPlants.length === 0 && !loading && (
              <div className="no-plants">
                <p>No plants match the current filter.</p>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
}

export default Dashboard;