import { useState, useEffect, useCallback, useRef } from 'react';
import PlantCard from '../components/PlantCard';
import Modal from '../components/Modal';
import PlantDetailModal from '../components/PlantDetailModal';
import AddPairingModal from '../components/AddPairingModal';
import NotificationBell from '../components/NotificationBell';
import NotificationModal from '../components/NotificationModal';
import { useNavigate } from 'react-router-dom';
import { usePlants } from '../hooks/usePlants';
import { useNotifications } from '../hooks/useNotifications';
import { useNotificationDropdown } from '../hooks/useNotificationDropdown';
import { usePlantFilter } from '../hooks/usePlantFilter';
import { usePlantModal } from '../hooks/usePlantModal';
import authService from '../services/authService';
import '../styles/Dashboard.css';

function Dashboard() {
  const navigate = useNavigate();
  
  // Plants logic
  const { plants, loading: plantsLoading, error: plantsError, fetchPlants, deletePlant } = usePlants();
  
  // Notifications logic
  const {
    notifications,
    unreadCount,
    loading: notificationsLoading,
    error: notificationsError,
    fetchNotifications,
    markAsRead,
    markAllAsRead
  } = useNotifications();
  
  // Notification dropdown UI
  const { isOpen, anchorRef, toggle, close } = useNotificationDropdown();
  
  // Plant filtering
  const { selectedFilter, setSelectedFilter, filteredPlants, stats } = usePlantFilter(plants);
  
  // Plant modals
  const {
    selectedPlant,
    showAddModal,
    openPlantDetails,
    closePlantDetails,
    openAddModal,
    closeAddModal
  } = usePlantModal();

  // Fetch notifications when dropdown opens
  useEffect(() => {
    if (isOpen) {
      fetchNotifications();
    }
  }, [isOpen, fetchNotifications]);

  const handleSignOut = () => {
    authService.logout();
    navigate('/landingpage');
  };

  const handlePairingSuccess = () => {
    fetchPlants();
    closeAddModal();
  };

  const handlePlantDeleted = (deletedPlantId) => {
    deletePlant(deletedPlantId);
    closePlantDetails();
  };

  return (
    <div className="dashboard">
      {/* Header */}
      <header className="dashboard-header">
        <h1>Plant Monitor Dashboard</h1>
        
        {/* Error State */}
        {plantsError && (
          <div className="error" style={{ color: 'red', padding: '1rem' }}>
            <p>Error: {plantsError}</p>
            <button onClick={fetchPlants}>Try again</button>
          </div>
        )}

        {/* Loading State */}
        {plantsLoading ? (
          <p>Loading plants...</p>
        ) : (
          <>
            {/* Summary Stats */}
            <div className="summary-stats">
              <div className="stat">
                <span className="stat-number">{stats.total}</span>
                <span className="stat-label">Total Plants</span>
              </div>
              <div className="stat healthy">
                <span className="stat-number">{stats.healthy}</span>
                <span className="stat-label">Healthy</span>
              </div>
              <div className="stat attention">
                <span className="stat-number">{stats.needsAttention}</span>
                <span className="stat-label">Need Attention</span>
              </div>
              
              <button 
                className="add-pairing-button" 
                onClick={openAddModal}
              >
                + Add Plant Pairing
              </button>
              
              {/* Notification Bell */}
              <div ref={anchorRef} className='relative'>
                <NotificationBell 
                  unreadCount={unreadCount}
                  onClick={toggle}
                />
                <NotificationModal
                  isOpen={isOpen}
                  onClose={close}
                  notifications={notifications}
                  loading={notificationsLoading}
                  error={notificationsError}
                  onMarkAsRead={markAsRead}
                  onMarkAllAsRead={markAllAsRead}
                  onRefresh={fetchNotifications}
                />
              </div>
            </div>
          </>
        )}
      </header>

      {/* Filter Controls */}
      <div className="filter-controls">
        <h3>Filter Plants:</h3>
        <div className="filter-buttons">
          <button 
            className={selectedFilter === 'all' ? 'active' : ''}
            onClick={() => setSelectedFilter('all')}
          >
            All Plants ({stats.total})
          </button>
          <button 
            className={selectedFilter === 'healthy' ? 'active' : ''}
            onClick={() => setSelectedFilter('healthy')}
          >
            Healthy ({stats.healthy})
          </button>
          <button 
            className={selectedFilter === 'needs-attention' ? 'active' : ''}
            onClick={() => setSelectedFilter('needs-attention')}
          >
            Needs Attention ({stats.needsAttention})
          </button>
          <button
            className="signout-button"
            onClick={handleSignOut}
          >
            Sign Out
          </button>
        </div>
      </div>

      {/* Plants Grid */}
      <div className="plants-grid">
        {plantsLoading ? (
          <div className="loading">Loading plants...</div>
        ) : plantsError ? (
          <div className="error">
            <p>Failed to load plants.</p>
            <button onClick={fetchPlants}>Retry</button>
          </div>
        ) : (
          <>
            {/* Map filtered plants to PlantCard components */}
            {filteredPlants.map(pairing => (
              <PlantCard 
                key={pairing.id}
                pairing={pairing}
                onClick={() => openPlantDetails(pairing)}
              />
            ))}
            
            {/* Empty state - no plants match filter */}
            {filteredPlants.length === 0 && !plantsLoading && (
              <div className="no-plants">
                <p>No plants match the current filter.</p>
              </div>
            )}
          </>
        )}
      </div>

      {/* Plant Details Modal */}
      <Modal isOpen={selectedPlant !== null} onClose={closePlantDetails}>
        {selectedPlant && (
          <PlantDetailModal 
            userPlant={selectedPlant}
            onDelete={handlePlantDeleted}
            onClose={closePlantDetails}
          />
        )}
      </Modal>

      {/* Add Pairing Modal */}
      <AddPairingModal
        isOpen={showAddModal}
        onClose={closeAddModal}
        onSuccess={handlePairingSuccess}
      />
    </div>
  );
}
export default Dashboard;