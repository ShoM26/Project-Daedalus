import React, { useState } from 'react';
import { CheckCheck } from 'lucide-react';
import Notification from './Notification';
import Modal from '../../../shared/components/Modal';
import styles from '../styles/NotificationModal.module.css';

function NotificationModal({ 
  isOpen, 
  onClose, 
  notifications, 
  loading,
  error,
  onMarkAsRead, 
  onMarkAllAsRead 
}) {
    const [showUnreadOnly, setShowUnreadOnly] = useState(false);

    const displayedNotifications = showUnreadOnly ?
      notifications.filter(n => !n.isRead)
      : notifications;

      const hasUnread = notifications.some(n => !n.isRead);
    return (
      <Modal isOpen={isOpen} onClose={onClose}>
      <div className={styles.modalBody}>
        <div className={styles.header}>
          <h3>Notifications</h3>
        </div>

        <div className={styles.controls}>
          <label className={styles.unreadFilter}>
            <input
              type="checkbox"
              checked={showUnreadOnly}
              onChange={(e) => setShowUnreadOnly(e.target.checked)}
            />
            Unread only
          </label>

          {hasUnread && (
            <button onClick={onMarkAllAsRead} className={styles.markAllRead}>
              <CheckCheck className={styles.iconSmall} />
              Mark all read
            </button>
          )}
        </div>

        <div className={styles.content}>
          {loading ? (
            <p className={styles.statusText}>Loading...</p>
          ) : error ? (
            <p className={styles.statusText}>Error loading notifications.</p>
          ) : displayedNotifications.length === 0 ? (
            <p className={styles.statusText}>No notifications.</p>
          ) : (
            displayedNotifications.map((n) => (
              <Notification
                key={n.notificationId}
                notification={n}
                onMarkAsRead={onMarkAsRead}
              />
            ))
          )}
        </div>
      </div>
    </Modal>
  );
}
export default NotificationModal;