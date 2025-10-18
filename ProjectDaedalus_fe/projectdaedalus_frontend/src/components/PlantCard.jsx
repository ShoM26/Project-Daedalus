// src/components/PlantCard.jsx
import { getStatusColor } from '../data/mockData';

function PlantCard({ pairing, onClick }) {
  const { plant, device, currentReading, status } = pairing;
  
  const isInIdealRange = () => {
    const { moistureLevel } = currentReading;
    return moistureLevel >= plant.idealMoistureMin && moistureLevel <= plant.idealMoistureMax;
  };

  return (
    <div className="plant-card"
    onClick={onClick} 
      style={{ cursor: 'pointer' }}>

      <div className="plant-header">
        <h3>{plant.familiarName}</h3>
        <span 
          className="status-indicator"
          style={{ backgroundColor: getStatusColor(status) }}
        >
          {status.replace('_', ' ')}
        </span>
      </div>

      <div className="device-info">
        <p><strong>Device:</strong> {device.id}</p>
        <p><strong>Battery:</strong> {currentReading.batteryLevel}%</p>
      </div>

      <div className="moisture-section">
        <div className="moisture-reading">
          <span className="moisture-value">{currentReading.moistureLevel}%</span>
          <span className="moisture-label">Soil Moisture</span>
        </div>
        
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

      <div className="plant-details">
        <p className="scientific-name">
          <em>{plant.scientificName}</em>
        </p>
        <p className="fun-fact">{plant.funFact}</p>
      </div>

      <div className="timestamp">
        Last reading: {new Date(currentReading.timestamp).toLocaleString()}
      </div>
    </div>
  );
}

export default PlantCard;