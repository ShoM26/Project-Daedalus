import { useState, useCallback, useEffect } from 'react';
import { plantService } from '../services/plantService';
import authService from '@auth/services/authService';

export function usePlants() {
  const [plants, setPlants] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchPlants = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const currentUser = authService.getCurrentUser();
      const apiPlants = await plantService.getUserPlants(currentUser.userId);

      const plantsWithReadings = await Promise.all(
        apiPlants.map(async (userPlant) => {
          let reading = null;

          if (userPlant.device && userPlant.device.deviceId) {
            try {
              reading = await plantService.getLatestReading(userPlant.device.deviceId);
            } catch (err) {
              if (reading !== null) {
                console.error(`Failed to get reading for device ${userPlant.device.deviceId}`);
              }
            }
          }

          return {
            ...userPlant,
            latestReading: reading
          };
        })
      );

      const transformedPlants = plantsWithReadings.map(userPlant => {
        const reading = userPlant.latestReading;
        const plant = userPlant.plant;
        
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
            timestamp: reading.timeStamp,
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
    } catch (err) {
      console.error('Failed to fetch plants:', err);
      setError('Failed to load plants. Please try again.');
    } finally {
      setLoading(false);
    }
  }, []);

  const deletePlant = useCallback((deletedPlantId) => {
    setPlants(prev => prev.filter(plant => plant.id !== deletedPlantId));
  }, []);

  // Initial fetch
  useEffect(() => {
    fetchPlants();
  }, [fetchPlants]);

  return {
    plants,
    loading,
    error,
    fetchPlants,
    deletePlant
  };
}