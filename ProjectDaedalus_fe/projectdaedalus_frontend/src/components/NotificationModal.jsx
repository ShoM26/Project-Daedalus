import React, { useState, useEffect } from 'react';
import { Bell, X, CheckCheck } from 'lucide-react';
import Notification from './Notification';
import '../styles/NotificationModal.css';

function NotificationModal({ 
  isOpen, 
  onClose, 
  notifications, 
  loading,
  error,
  onMarkAsRead, 
  onMarkAllAsRead, 
  onRefresh 
}) {
    const [showUnreadOnly, setShowUnreadOnly] = useState(false);

    if(!isOpen) return null;

    const displayedNotifications = showUnreadOnly ?
      notifications.filter(n => !n.isRead)
      : notifications;

      const hasUnread = notifications.some(n => !n.isRead);
    return (
    <>
      {/* Backdrop overlay */}
      <div className="notification-modal-backdrop" onClick={onClose} />
      
      {/* Modal popup */}
      <div className="notification-modal-popup">
        {/* Header */}
        <div className="notification-modal-header">
          <h3>Notifications</h3>
          <button onClick={onClose} className="close-button">
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Controls */}
        <div className="notification-modal-controls">
          <label className="unread-filter">
            <input
              type="checkbox"
              checked={showUnreadOnly}
              onChange={(e) => setShowUnreadOnly(e.target.checked)}
            />
            Unread only
          </label>
          
          {hasUnread && (
            <button onClick={onMarkAllAsRead} className="mark-all-read">
              <CheckCheck className="w-4 h-4" />
              Mark all read
            </button>
          )}
        </div>

        {/* Content */}
        <div className="notification-modal-content">
          {/* ... your existing content rendering ... */}
        </div>
      </div>
    </>
  );
};
export default NotificationModal