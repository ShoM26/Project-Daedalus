import { useEffect } from 'react';
import Modal from '../../../shared/components/Modal.jsx';
import authService from '../../auth/services/authService.js';

function RegisterDeviceModal({ isOpen, onClose}){
    const currentUser = authService.getCurrentUser();
    useEffect(() => {
        if(isOpen){
            //Display Diagram
        }
    }, [isOpen]);
    return (
        <Modal isOpen={isOpen} onClose={onClose}>
            <div className="register-body">
                <h1>Connect Your Device</h1>
                <p>1. Ensure your device is plugged</p>
                <p>2. Ensure somethign else is happening</p>
            </div>
        </Modal>
    );
}

export default RegisterDeviceModal;