import { useState, useEffect, useCallback, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
//components
import Modal from '@shared/components/Modal';
import PlantCard from '../components/PlantCard';
import PlantDetailModal from '../components/PlantDetailModal';
import AddPairingModal from '../components/AddPairingModal';
import NotificationBell from '@notificationmodal/components/NotificationBell';
import NotificationModal from '@notificationmodal/components/NotificationModal';
//hooks
import { usePlants } from '../hooks/usePlants';
import { useNotifications } from '../hooks/useNotifications';
import { useNotificationDropdown } from '../hooks/useNotificationDropdown';
import { usePlantFilter } from '../hooks/usePlantFilter';
import { usePlantModal } from '../hooks/usePlantModal';
//services
import authService from '@auth/services/authService';
//styles
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
      {/* Top Navigation Bar */}
      <nav className="dashboard-nav">
        <h1>Plant Monitor Dashboard</h1>
        
        <div className="nav-actions">
          <button 
            className="add-pairing-button" 
            onClick={openAddModal}
          >
            + Add Plant
          </button>
          
          {/* Notification Bell */}
          <div ref={anchorRef} className='relative'>
            <NotificationBell 
              unreadCount={unreadCount}
              onClick={toggle}
            />
            <Modal>
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
            </Modal>
            
          </div>
          
          <button
            className="signout-button"
            onClick={handleSignOut}
          >
            Sign Out
          </button>
        </div>
      </nav>

      {/* Main Content */}
      <main className="dashboard-main">
        {/* Error State */}
        {plantsError && (
          <div className="error-banner">
            <p>Error: {plantsError}</p>
            <button onClick={fetchPlants}>Try again</button>
          </div>
        )}

        {/* Loading State */}
        {plantsLoading ? (
          <div className="loading-container">
            <p>Loading plants...</p>
          </div>
        ) : (
          <>
            {/* Summary Stats Cards */}
            <div className="summary-stats">
              <div className="stat-card">
                <span className="stat-number">{stats.total}</span>
                <span className="stat-label">Total Plants</span>
              </div>
              <div className="stat-card healthy">
                <span className="stat-number">{stats.healthy}</span>
                <span className="stat-label">Healthy</span>
              </div>
              <div className="stat-card attention">
                <span className="stat-number">{stats.needsAttention}</span>
                <span className="stat-label">Need Attention</span>
              </div>
            </div>

            {/* Filter Controls */}
            <div className="filter-controls">
              <h3>Filter Plants</h3>
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
              </div>
            </div>

            {/* Plants Grid */}
            <div className="plants-grid">
              {filteredPlants.map(pairing => (
                <PlantCard 
                  key={pairing.id}
                  pairing={pairing}
                  onClick={() => openPlantDetails(pairing)}
                />
              ))}
              
              {/* Empty state */}
              {filteredPlants.length === 0 && (
                <div className="no-plants">
                  <p>No plants match the current filter.</p>
                </div>
              )}
            </div>
          </>
        )}
      </main>

      {/* Modals */}
      <Modal isOpen={selectedPlant !== null} onClose={closePlantDetails}>
        {selectedPlant && (
          <PlantDetailModal 
            userPlant={selectedPlant}
            onDelete={handlePlantDeleted}
            onClose={closePlantDetails}
          />
        )}
      </Modal>
      <Modal>
        <AddPairingModal
        isOpen={showAddModal}
        onClose={closeAddModal}
        onSuccess={handlePairingSuccess}
      /></Modal>
      
    </div>
  );
}
export default Dashboard;