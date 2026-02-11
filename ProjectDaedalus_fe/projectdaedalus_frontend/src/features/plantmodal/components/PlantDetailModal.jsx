import { useState } from 'react';
import SensorChart from './SensorChart';
import '../styles/PlantDetailModal.css';
import ConfirmDialog from '../../dashboard/components/ConfirmDialog';


function PlantDetailModal({ userPlant, onDelete, onClose }) {
  
  const { plant, device, currentReading } = userPlant;
  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState(null);
  const [showConfirm, setShowConfirm] = useState(false);

  const handleDelete = async () => {
      setShowConfirm(true);
    };

    const handleConfirmedDelete = async () => {
      setShowConfirm(false);
      setDeleting(true);
      setDeleteError(null);

      try {
        await plantService.deleteUserPlant(userPlant.id);
        onDelete(userPlant.id);
      } catch (err) {
        setDeleteError(err.message);
        setDeleting(false);
      }
    };

    const handleCancelDelete = () => {
      setShowConfirm(false);
    };

  return (
    <div className="plant-detail-modal">
      <div className="modal-header">
        <div>
          <h2 className="familiar-name">{plant.familiarName}</h2>
          <p className="scientific-name">{plant.scientificName}</p>
        </div>
      </div>

      {deleteError && (
        <div className="error-message">
          Error: {deleteError}
        </div>
      )}
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
          <p className="large-number">{device.id}</p>
        </div>
      </div>
      {plant.funFact && (
        <div className="fun-fact">
          <h3>🌱 Did you know?</h3>
          <p>{plant.funFact}</p>
        </div>
      )}

      <div className="chart-section">
        <SensorChart 
        deviceId={device.id}
        moistureMin={plant.idealMoistureMin}
        moistureMax={plant.idealMoistureMax} />
      </div>
        <div className="modal-footer"><button 
          className="delete-button" 
          onClick={handleDelete}
          disabled={deleting}
          >
          {deleting ? 'Deleting...' : 'Delete Plant'}
        </button>
      </div>
       <ConfirmDialog
        isOpen={showConfirm}
        onConfirm={handleConfirmedDelete}
        onCancel={handleCancelDelete}
        title="Delete Plant"
        message={`Are you sure you want to remove ${plant.familiarName} from your dashboard? This action cannot be undone.`}
      />
    </div>
  );
}

export default PlantDetailModal;