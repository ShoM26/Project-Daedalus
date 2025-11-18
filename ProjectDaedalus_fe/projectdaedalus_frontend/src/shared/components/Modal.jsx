// src/components/Modal.jsx
import React from 'react';
import styles from '../styles/Modal.module.css';

function Modal({ isOpen, onClose, children }) {
  // Don't render anything if modal isn't open
  if (!isOpen) return null;

  // Close modal when clicking the overlay (background)
  const handleOverlayClick = (e) => {
    // Only close if clicking the overlay itself, not the content
    if (e.target === e.currentTarget) onClose();
  };

  return (
    <div className={styles.overlay} onClick={handleOverlayClick}>
      <div className={styles.content}>
        <button className={styles.close} onClick={onClose}>
          ×
        </button>
        {children}
      </div>
    </div>
  );
}

export default Modal;