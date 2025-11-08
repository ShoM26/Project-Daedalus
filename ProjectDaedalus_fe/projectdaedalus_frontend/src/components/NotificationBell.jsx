import React from "react";
import { Bell } from 'lucide-react';

function NotificationBell({ unreadCount, onClick }) {
  return (
    <button
      onClick={onClick}
      className="relative p-2 rounded-lg hover:bg-gray-100 transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500"
      aria-label={`Notifications ${unreadCount > 0 ? `(${unreadCount} unread)` : ''}`}
    >
      <Bell className="w-6 h-6 text-gray-700" />
      {unreadCount > 0 && (
        <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs font-bold rounded-full min-w-[20px] h-5 flex items-center justify-center px-1">
          {unreadCount > 99 ? '99+' : unreadCount}
        </span>
      )}
    </button>
  );
};
export default NotificationBell;