import React from "react";
import { Bell } from 'lucide-react';
import styles from '../styles/NotificationBell.module.css';

function NotificationBell({ unreadCount, onClick }) {
  const hasUnread = unreadCount && unreadCount > 0;
  return (
    <button
      onClick={onClick}
      className={styles.bellButton}
      aria-label={`Notifications ${hasUnread ? `(${unreadCount} unread)` : ''}`}
    >
      <Bell className={styles.bellIcon} />
      {hasUnread && (
        <span className={styles.badge}>
          {unreadCount > 99 ? '99+' : unreadCount}
        </span>
      )}
    </button>
  );
}
export default NotificationBell;