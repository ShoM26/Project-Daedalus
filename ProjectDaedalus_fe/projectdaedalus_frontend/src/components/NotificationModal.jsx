import React, { useState, useEffect } from 'react';
import { Bell, X, CheckCheck } from 'lucide-react';
import Notification from './Notification';

function NotificationModal({ isOpen, onClose, notifications, loading, onMarkAsRead, onMarkAllAsRead, onRefresh, anchorRef }) {
        const [showUnreadOnly, setShowUnreadOnly] = useState(false);

    useEffect(() => {
    if (isOpen) {
      onRefresh(showUnreadOnly);
    }
  }, [isOpen, showUnreadOnly, onRefresh]);

  // Close on outside click
  useEffect(() => {
    if (!isOpen) return;

    const handleClickOutside = (event) => {
      if (anchorRef?.current && !anchorRef.current.contains(event.target)) {
        onClose();
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [isOpen, onClose, anchorRef]);

  useEffect(() =>{
    if (!isOpen) return;
    const handleEscape = (event) =>{
        if(event.key === 'Escape'){
            onClose();
        }
    };
    document.addEventListener('keydown', handleEscape);
    return () => document.removeEventListener('keydown', handleEscape);
  }, [isOpen, onClose]);
  
  if (!isOpen) return null;

  const hasUnread = notifications.some(n => !n.isRead);

  return (
    <div className="absolute right-0 top-full mt-2 w-96 bg-white rounded-lg shadow-2xl border border-gray-200 z-50 max-h-[600px] flex flex-col">
      {/* Header */}
      <div className="p-4 border-b border-gray-200 flex items-center justify-between bg-gray-50 rounded-t-lg">
        <h3 className="font-semibold text-gray-900 text-lg">Notifications</h3>
        <button
          onClick={onClose}
          className="p-1 hover:bg-gray-200 rounded transition-colors"
          aria-label="Close"
        >
          <X className="w-5 h-5 text-gray-600" />
        </button>
      </div>

      {/* Controls */}
      <div className="p-3 border-b border-gray-200 flex items-center justify-between bg-white">
        <label className="flex items-center gap-2 text-sm text-gray-700 cursor-pointer">
          <input
            type="checkbox"
            checked={showUnreadOnly}
            onChange={(e) => setShowUnreadOnly(e.target.checked)}
            className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
          />
          Unread only
        </label>
        
        {hasUnread && (
          <button
            onClick={markAllAsRead}
            className="flex items-center gap-1 text-sm text-blue-600 hover:text-blue-700 font-medium"
          >
            <CheckCheck className="w-4 h-4" />
            Mark all read
          </button>
        )}
      </div>

      {/* Content */}
      <div className="flex-1 overflow-y-auto">
        {loading ? (
          <div className="p-8 text-center text-gray-500">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto mb-2"></div>
            Loading notifications...
          </div>
        ) : notifications.length === 0 ? (
          <div className="p-8 text-center text-gray-500">
            <Bell className="w-12 h-12 mx-auto mb-2 text-gray-300" />
            <p>No notifications</p>
            {showUnreadOnly && (
              <p className="text-sm mt-1">You're all caught up!</p>
            )}
          </div>
        ) : (
          notifications.map(notification => (
            <Notification
              key={notification.id}
              notification={notification}
              onMarkAsRead={markAsRead}
            />
          ))
        )}
      </div>
    </div>
  );
};
export default NotificationModal