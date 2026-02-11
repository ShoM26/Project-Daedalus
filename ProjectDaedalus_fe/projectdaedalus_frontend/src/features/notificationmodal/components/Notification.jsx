import { useState } from 'react';
import { Check, Clock } from 'lucide-react';
import { formatTimeAgo } from '../../../shared/utils/dateUtils';
import styles from '../styles/Notification.module.css';

function Notification({ notification, onMarkAsRead }) {
    const [isMarking, setIsMarking] = useState(false);

    const handleMarkAsRead = async () => {
        setIsMarking(true);
        await onMarkAsRead(notification.notificationId);
        setIsMarking(false);
    };

    return (
    <div
      className={`${styles.notification} ${
        !notification.isRead ? styles.unread : ''
      }`}
    >
      <div className={styles.contentWrapper}>
        <div className={styles.textSection}>
          <div className={styles.headerRow}>
            {!notification.isRead && (
              <span className={styles.unreadDot} />)}
            <p className={styles.plantName}>
              {notification.plantName}
            </p>
          </div>
          
          <p className={styles.message}>
            {notification.message}
          </p>
          
          <div className={styles.metaRow}>
            <span className={styles.metaItem}>
              <Clock className={styles.icon} />
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
            className={styles.markReadButton}
            aria-label="Mark as read"
            title="Mark as read"
          >
            <Check className={styles.checkIcon} />
          </button>
        )}
      </div>
    </div>
  );
}

export default Notification;