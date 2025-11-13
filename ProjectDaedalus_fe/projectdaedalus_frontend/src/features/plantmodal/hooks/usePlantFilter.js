import { useState, useMemo } from 'react';

export function usePlantFilter(plants) {
  const [selectedFilter, setSelectedFilter] = useState('all');

  const filteredPlants = useMemo(() => {
    return plants.filter(plant => {
      if (selectedFilter === 'all') return true;
      if (selectedFilter === 'needs-attention') return plant.status !== 'healthy';
      return plant.status === selectedFilter;
    });
  }, [plants, selectedFilter]);

  const stats = useMemo(() => ({
    healthy: plants.filter(p => p.status === 'healthy').length,
    needsAttention: plants.filter(p => p.status !== 'healthy').length,
    total: plants.length
  }), [plants]);

  return {
    selectedFilter,
    setSelectedFilter,
    filteredPlants,
    stats
  };
}