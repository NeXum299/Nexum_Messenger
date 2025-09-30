import { useState, useEffect } from 'react';
import styles from '../Modal.module.css';
import { api } from '../../../../../../../services/api.js';

export default function LeaveGroup({ onClose, isOpen }) {
    const [error, setError] = useState('');
    const [modalState, setModalState] = useState('');
    const [isLeaving, setIsLeaving] = useState(false);

    useEffect(() => {
        if (isOpen) {
            setModalState('open');
        } else if (modalState === 'open') {
            setModalState('closing');
            const timer = setTimeout(() => setModalState(''), 300);
            return () => clearTimeout(timer);
        }
    }, [isOpen, modalState]);

    const handleLeave = async () => {
        setIsLeaving(true);
        setError('');

        try {
            await api.post('');
            if (onClose) onClose();
        }
        catch (error) {
            console.error('Error leaving group:', error);
            setError(error.response?.data?.message ||
                error.response?.data?.errors?.join(', ') ||
                    'Ошибка при выходе из группы');
        } finally {
            setIsLeaving(false);
        }
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    if (!isOpen && modalState === '') return null;

    return (
        <div className={`${styles.modalOverlay} ${modalState ? styles[modalState] : ''}`}>
            <div className={styles.modalContent}>
                <button className={styles.closeButton} onClick={onClose}>&times;</button>
                <h2 className={styles.headerText}>Покинуть группу</h2>

                {error && <div className={styles.error}>{error}</div>}

                <div className={styles.formInput}>
                    <p style={{ color: '#fff', marginBottom: '20px', textAlign: 'center' }}>
                        Вы уверены, что хотите покинуть группу? Это действие нельзя отменить.
                    </p>
                    
                    <div style={{ display: 'flex', gap: '10px', justifyContent: 'center' }}>
                        <button 
                            className={styles.modalButton} 
                            onClick={onClose}
                            style={{ background: '#666' }}
                        >
                            Отмена
                        </button>
                        
                        <button 
                            className={styles.modalButton} 
                            onClick={handleLeave}
                            disabled={isLeaving}
                            style={{ background: '#e74c3c' }}
                        >
                            {isLeaving ? 'Выход...' : 'Покинуть группу'}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
