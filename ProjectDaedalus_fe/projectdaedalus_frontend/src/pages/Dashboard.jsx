import { useState, useEffect } from 'react';
import PlantCard from '../components/PlantCard';
import { plantService } from '../services/plantService';
import  authService  from '../services/authService';

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
      const currentUser = authService.getCurrentUser();
      const apiPlants = await plantService.getUserPlants(currentUser.userId);
      
      const plantsWithReadings = await Promise.all(
        apiPlants.map(async (userPlant) => {
          let reading = null;

          if (userPlant.device && userPlant.device.deviceId) {
            try {
              reading = await plantService.getLatestReading(userPlant.device.deviceId);
            } catch (err) {
              console.error(`Failed to get reading for device ${userPlant.device.deviceId}`);
            }
          }

          return {
            ...userPlant,
            latestReading: reading
          };
        })
      );

      // Transform API data to match component expectations
      const transformedPlants = plantsWithReadings.map(userPlant => {
        const reading = userPlant.latestReading;
        const plant = userPlant.plant;
        
        // Calculate status based on reading
        let status = "offline";
        if (reading && reading.moistureLevel != null) {
          if (reading.moistureLevel > plant.moistureHighRange) {
            status = "overwatered";
          } else if (reading.moistureLevel < plant.moistureLowRange) {
            status = "needs_water";
          } else {
            status = "healthy";
          }
        }

        return {
          id: userPlant.userPlantId,
          plant: {
            id: plant.plantId,
            scientificName: plant.scientificName,
            familiarName: plant.familiarName,
            idealMoistureMin: plant.moistureLowRange,
            idealMoistureMax: plant.moistureHighRange,
            funFact: plant.funFact || "This plant is part of your monitoring system!"
          },
          device: userPlant.device ? {
            id: userPlant.device.deviceId,
            name: userPlant.device.deviceName,
            connectionType: userPlant.device.connectionType,
            connectionAddress: userPlant.device.connectionAddress
          } : {
            id: null,
            name: "No device assigned",
            connectionType: "None",
            connectionAddress: "None"
          },
          currentReading: reading ? {
            moistureLevel: reading.moistureLevel,
            timestamp: reading.timestamp,
            batteryLevel: reading.batteryLevel || 100
          } : {
            moistureLevel: 0,
            timestamp: null,
            batteryLevel: 0
          },
          status: status
        };
      });
      
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