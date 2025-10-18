import { useState, useEffect } from 'react';
import Modal from './Modal';
import ConfirmDialog from './ConfirmDialog';
import { plantService } from '../services/plantService';
import '../styles/AddPairingModal.css';


function AddPairingModal({ isOpen, onClose, onSuccess }) {
  const [devices, setDevices] = useState([]);
  const [plants, setPlants] = useState([]);
  const [existingPairings, setExistingPairings] = useState([]);
  
  const [selectedDeviceId, setSelectedDeviceId] = useState('');
  const [selectedPlantId, setSelectedPlantId] = useState('');
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [showConfirm, setShowConfirm] = useState(false);
  const [confirmMessage, setConfirmMessage] = useState('');

  const userId = JSON.parse(localStorage.getItem('user')).userId;

  useEffect(() => {
    if (isOpen) {
      fetchModalData();
    }
  }, [isOpen]);

  const fetchModalData = async () => {
    setLoading(true);
    setError(null);
    
    try {
      const [devicesData, plantsData, pairingsData] = await Promise.all([
        plantService.getDevicesByUser(userId),
        plantService.getAllPlants(),
        plantService.getUserPlants(userId)
      ]);
      
      setDevices(devicesData);
      setPlants(plantsData);
      setExistingPairings(pairingsData);
    } catch (err) {
      setError('Failed to load data: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const getExistingPairing = (deviceId) => {
    return existingPairings.find(p => p.deviceId === parseInt(deviceId));
  };

  // Helper to get plant name by id
  const getPlantName = (plantId) => {
    const plant = plants.find(p => p.plantId === plantId);
    return plant ? plant.familiarName : 'Unknown Plant';
  };

  const handleSave = async () => {
    if (!selectedDeviceId || !selectedPlantId) {
      setError('Please select both a device and a plant');
      return;
    }

    const existingPairing = getExistingPairing(selectedDeviceId);

    if (existingPairing) {
      const currentPlantName = getPlantName(existingPairing.plantId);
      const newPlantName = getPlantName(parseInt(selectedPlantId));
      
      setConfirmMessage(
        `Device ${selectedDeviceId} is currently paired with ${currentPlantName}. ` +
        `Changing to ${newPlantName} will delete all historical sensor data for this device. ` +
        `Are you sure?`
      );
      setShowConfirm(true);
    } else {
      await createPairing();
    }
  };

  const handleConfirmedSave = async () => {
    setShowConfirm(false);
    await updatePairing();
  };

  const createPairing = async () => {
    setLoading(true);
    setError(null);

    try {
      const result = await plantService.postUserPlant(
        userId,
        parseInt(selectedDeviceId),
        parseInt(selectedPlantId)
      );

      if (result.success) {
        onSuccess();
        resetAndClose();
      } else {
        setError(result.message || 'Failed to create pairing');
      }
    } catch (err) {
      setError('Error creating pairing: ' + err.message);
    } finally {
      setLoading(false);
    }
  };

  const updatePairing = async () => {
    setLoading(true);
    setError(null);
    try {

      try {
        await plantService.deleteSensorReadingsByDevice(parseInt(selectedDeviceId));
      } catch(deleteError){
        console.log('No sensor readings to delete or delete failed:', deleteError);
      }

        const updateResult = await plantService.putUserPlant(
        parseInt(selectedDeviceId),
        userId,
        parseInt(selectedPlantId)
      );

      if (updateResult.success) {

        onSuccess(); // Refresh dashboard
        resetAndClose();
      } else {
        setError(updateResult.message || 'Failed to update pairing');
      }
      } catch (err) {
        setError('Error updating pairing: ' + err.message);
      } finally {
        setLoading(false);
      }
  };

  const resetAndClose = () => {
    setSelectedDeviceId('');
    setSelectedPlantId('');
    setError(null);
    onClose();
  };

  return (
    <>
      <Modal isOpen={isOpen} onClose={resetAndClose}>
        <div className="add-pairing-modal">
          <h2>Add Plant Pairing</h2>

          {error && (
            <div className="error-message">
              {error}
            </div>
          )}

          {loading && <p>Loading...</p>}

          <div className="form-group">
            <label htmlFor="device-select">Select Device:</label>
            <select
              id="device-select"
              value={selectedDeviceId}
              onChange={(e) => setSelectedDeviceId(e.target.value)}
              disabled={loading}
            >
              <option value="">-- Choose a device --</option>
              {devices.map((device) => {
                const pairing = getExistingPairing(device.deviceId);
                const label = pairing
                  ? `Device ${device.deviceId} (currently: ${getPlantName(pairing.plantId)})`
                  : `Device ${device.deviceId}`;
                
                return (
                  <option key={device.deviceId} value={device.deviceId}>
                    {label}
                  </option>
                );
              })}
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="plant-select">Select Plant:</label>
            <select
              id="plant-select"
              value={selectedPlantId}
              onChange={(e) => setSelectedPlantId(e.target.value)}
              disabled={loading}
            >
              <option value="">-- Choose a plant --</option>
              {plants.map((plant) => (
                <option key={plant.plantId} value={plant.plantId}>
                  {plant.familiarName} ({plant.scientificName})
                </option>
              ))}
            </select>
          </div>

          <div className="modal-actions">
            <button 
              onClick={handleSave} 
              disabled={loading || !selectedDeviceId || !selectedPlantId}
              className="save-button"
            >
              {loading ? 'Saving...' : 'Save Pairing'}
            </button>
            <button 
              onClick={resetAndClose} 
              disabled={loading}
              className="cancel-button"
            >
              Cancel
            </button>
          </div>
        </div>
      </Modal>

      <ConfirmDialog
        isOpen={showConfirm}
        onConfirm={handleConfirmedSave}
        onCancel={() => setShowConfirm(false)}
        title="Confirm Pairing Change"
        message={confirmMessage}
      />
    </>
  );
}

export default AddPairingModal;