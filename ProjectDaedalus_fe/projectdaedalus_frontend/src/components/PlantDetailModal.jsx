// src/components/PlantDetailModal.jsx
import React, { useState } from 'react';
import SensorChart from './SensorChart';
import '../styles/PlantDetailModal.css';

function PlantDetailModal({ userPlant, onDelete, onClose }) {
console.log('Full userPlant object:', userPlant);
const { plant, device, currentReading } = userPlant;
  console.log('Device object:', device);
  console.log('Device.deviceId:', device?.deviceId)

  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState(null);

  // Destructure for cleaner code
  

  // Handler for delete button
  const handleDelete = async () => {
    // Confirm before deleting
    if (!window.confirm(`Are you sure you want to remove ${plant.familiarName} from your dashboard?`)) {
      return;
    }

    setDeleting(true);
    setDeleteError(null);

    try {
      await apiService.delete(`/UserPlants/${userPlant.id}`);
      onDelete(userPlant.id);
      }catch(err){
        setDeleteError(err.message);
        setDeleting(false);
    }
};

  return (
    <div className="plant-detail-modal">
      {/* Header Section */}
      <div className="modal-header">
        <div>
          <h2>{plant.familiarName}</h2>
          <p className="scientific-name">{plant.scientificName}</p>
        </div>
        <button 
          className="delete-button" 
          onClick={handleDelete}
          disabled={deleting}
        >
          {deleting ? 'Deleting...' : 'Delete Plant'}
        </button>
      </div>

      {deleteError && (
        <div className="error-message">
          Error: {deleteError}
        </div>
      )}

      {/* Plant Info Grid */}
      <div className="plant-info-grid">
        <div className="info-card">
          <h3>Current Moisture</h3>
          <p className="large-number">
            {currentReading ? `${currentReading.moistureLevel}%` : 'No data'}
          </p>
        </div>

        <div className="info-card">
          <h3>Ideal Range</h3>
          <p className="large-number">
            {plant.idealMoistureMin}% - {plant.idealMoistureMax}%
          </p>
        </div>

        <div className="info-card">
          <h3>Device</h3>
          <p>{device.id}</p>
        </div>
      </div>

      {/* Fun Fact Section */}
      {plant.funFact && (
        <div className="fun-fact">
          <h3>🌱 Did you know?</h3>
          <p>{plant.funFact}</p>
        </div>
      )}

      {/* Chart Section - we'll build this next */}
      <div className="chart-section">
        <h3>Moisture History</h3>
        <SensorChart 
        deviceId={device.id}
        moistureMin={plant.idealMoistureMin}
        moistureMax={plant.idealMoistureMax} />
      </div>
    </div>
  );
}

export default PlantDetailModal;