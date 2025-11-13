/**
 * Check if moisture level is in ideal range
 * @param {number} moistureLevel - Current moisture percentage
 * @param {number} idealMin - Minimum ideal moisture
 * @param {number} idealMax - Maximum ideal moisture
 * @returns {boolean}
 */
export const isInIdealRange = (moistureLevel, idealMin, idealMax) => {
  return moistureLevel >= idealMin && moistureLevel <= idealMax;
};

/**
 * Format battery level with warning indicator
 * @param {number} batteryLevel - Battery percentage
 * @returns {object} { value: number, isLow: boolean }
 */
export const formatBatteryLevel = (batteryLevel) => {
  return {
    value: batteryLevel,
    isLow: batteryLevel < 20
  };
};