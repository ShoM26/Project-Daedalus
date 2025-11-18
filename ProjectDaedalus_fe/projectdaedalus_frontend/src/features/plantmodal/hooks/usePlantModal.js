import { useState } from 'react';

export function usePlantModal() {
  const [selectedPlant, setSelectedPlant] = useState(null);
  const [showAddModal, setShowAddModal] = useState(false);

  const openPlantDetails = (plant) => setSelectedPlant(plant);
  const closePlantDetails = () => setSelectedPlant(null);
  const openAddModal = () => setShowAddModal(true);
  const closeAddModal = () => setShowAddModal(false);

  return {
    selectedPlant,
    showAddModal,
    openPlantDetails,
    closePlantDetails,
    openAddModal,
    closeAddModal
  };
}