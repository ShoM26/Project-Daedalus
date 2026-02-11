import { useEffect } from 'react';
import Modal from '../../../shared/components/Modal.jsx';

function RegisterDeviceModal({ isOpen, onClose}){
    useEffect(() => {
        if(isOpen){
        }
    }, [isOpen]);
    return (
        <Modal isOpen={isOpen} onClose={onClose}>
            <div className="register-body">
                <h1>Connect Your Device</h1>
                <p>1. Ensure your device is plugged</p>
                <p>2. Ensure the bridge .exe file is running</p>
            </div>
        </Modal>
    );
}

export default RegisterDeviceModal;