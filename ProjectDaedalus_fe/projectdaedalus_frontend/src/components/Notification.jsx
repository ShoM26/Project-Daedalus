import React, { useState } from 'react';
import { Check, Clock } from 'lucide-react';
import { formatTimeAgo } from '../utils/dateUtils';

function Notification({ notification, onMarkAsRead }) {
    const [isMarking, setIsMarking] = useState(false);

    const handleMarkAsRead = async () => {
        setIsMarking(true);
        await onMarkingAsRead(notification.id);
        setIsMarking(false);
    };

    return (
    <div
      className={`p-4 border-b border-gray-200 hover:bg-gray-50 transition-colors ${
        !notification.isRead ? 'bg-blue-50' : ''
      }`}
    >
      <div className="flex items-start justify-between gap-3">
        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-2 mb-1">
            {!notification.isRead && (
              <span className="w-2 h-2 bg-blue-500 rounded-full flex-shrink-0" />
            )}
            <p className="font-semibold text-gray-900 truncate">
              {notification.plantName}
            </p>
          </div>
          
          <p className="text-sm text-gray-700 mb-2">
            {notification.message}
          </p>
          
          <div className="flex items-center gap-4 text-xs text-gray-500">
            <span className="flex items-center gap-1">
              <Clock className="w-3 h-3" />
              {formatTimeAgo(notification.createdAt)}
            </span>
            {notification.moisturePercentage !== undefined && (
              <span>Moisture: {notification.moisturePercentage}%</span>
            )}
          </div>
        </div>

        {!notification.isRead && (
          <button
            onClick={handleMarkAsRead}
            disabled={isMarking}
            className="flex-shrink-0 p-2 text-blue-600 hover:bg-blue-100 rounded-lg transition-colors disabled:opacity-50"
            aria-label="Mark as read"
            title="Mark as read"
          >
            <Check className="w-4 h-4" />
          </button>
        )}
      </div>
    </div>
  );
};

export default Notification;