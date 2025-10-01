// src/components/PlantCard.jsx
import { getStatusColor } from '../data/mockData';

// This is a React COMPONENT - a reusable piece of UI
function PlantCard({ pairing }) {
  // DESTRUCTURING - extract data from the pairing object for easier use
  const { plant, device, currentReading, status } = pairing;
  
  // HELPER FUNCTION - calculate if moisture is in ideal range
  const isInIdealRange = () => {
    const { moistureLevel } = currentReading;
    return moistureLevel >= plant.idealMoistureMin && moistureLevel <= plant.idealMoistureMax;
  };

  // JSX - this looks like HTML but it's actually JavaScript
  return (
    <div className="plant-card">
      {/* Plant Name Header */}
      <div className="plant-header">
        <h3>{plant.familiarName}</h3>
        <span 
          className="status-indicator"
          style={{ backgroundColor: getStatusColor(status) }}
        >
          {status.replace('_', ' ')}
        </span>
      </div>

      {/* Device Info */}
      <div className="device-info">
        <p><strong>Device:</strong> {device.id}</p>
        <p><strong>Battery:</strong> {currentReading.batteryLevel}%</p>
      </div>

      {/* Moisture Reading - the main data point */}
      <div className="moisture-section">
        <div className="moisture-reading">
          <span className="moisture-value">{currentReading.moistureLevel}%</span>
          <span className="moisture-label">Soil Moisture</span>
        </div>
        
        {/* Ideal Range Bar */}
        <div className="ideal-range">
          <span>Ideal: {plant.idealMoistureMin}% - {plant.idealMoistureMax}%</span>
          <div className="range-bar">
            <div 
              className="current-level"
              style={{ 
                width: `${Math.min(currentReading.moistureLevel, 100)}%`,
                backgroundColor: isInIdealRange() ? '#22c55e' : '#ef4444'
              }}
            />
          </div>
        </div>
      </div>

      {/* Scientific name and fun fact */}
      <div className="plant-details">
        <p className="scientific-name">
          <em>{plant.scientificName}</em>
        </p>
        <p className="fun-fact">{plant.funFact}</p>
      </div>

      {/* Last updated timestamp */}
      <div className="timestamp">
        Last reading: {new Date(currentReading.timestamp).toLocaleString()}
      </div>
    </div>
  );
}

// EXPORT - makes this component available to other files
export default PlantCard;