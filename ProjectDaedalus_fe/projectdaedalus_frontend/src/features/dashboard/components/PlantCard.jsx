// src/features/plants/components/PlantCard.jsx
import { getStatusColor, isInIdealRange } from '../utils/plantUtils';
import styles from '../styles/PlantCard.module.css';

/**
 * PlantCard Component
 * Displays a plant pairing with its device, moisture readings, and status
 * 
 * @param {Object} pairing - The plant-device pairing data
 * @param {Function} onClick - Handler for card click
 */
function PlantCard({ pairing, onClick }) {
  const { plant, device, currentReading, status } = pairing;
  
  const inIdealRange = isInIdealRange(
    currentReading.moistureLevel,
    plant.idealMoistureMin,
    plant.idealMoistureMax
  );

  return (
    <div 
      className={styles.plantCard}
      onClick={onClick}
    >
      {/* Plant Name Header */}
      <div className={styles.plantHeader}>
        <h3>{plant.familiarName}</h3>
        <span 
          className={styles.statusIndicator}
          style={{ backgroundColor: getStatusColor(status) }}
        >
          {status.replace('_', ' ')}
        </span>
      </div>

      
      <div className={styles.deviceInfo}>
        <p><strong>Device:</strong> {device.id}</p>
        <p><strong>Battery:</strong> {currentReading.batteryLevel}%</p>
      </div>

      
      <div className={styles.moistureSection}>
        <div className={styles.moistureReading}>
          <span className={styles.moistureValue}>
            {currentReading.moistureLevel}%
          </span>
          <span className={styles.moistureLabel}>Soil Moisture</span>
        </div>
        
        
        <div className={styles.idealRange}>
          <span>
            Ideal: {plant.idealMoistureMin}% - {plant.idealMoistureMax}%
          </span>
          <div className={styles.rangeBar}>
            <div 
              className={styles.currentLevel}
              style={{ 
                width: `${Math.min(currentReading.moistureLevel, 100)}%`,
                backgroundColor: inIdealRange ? '#22c55e' : '#ef4444'
              }}
            />
          </div>
        </div>
      </div>

      
      <div className={styles.plantDetails}>
        <p className={styles.scientificName}>
          <em>{plant.scientificName}</em>
        </p>
        <p className={styles.funFact}>{plant.funFact}</p>
      </div>

     
      <div className={styles.timestamp}>
        Last reading: {new Date(currentReading.timestamp).toLocaleString()}
      </div>
    </div>
  );
}

export default PlantCard;