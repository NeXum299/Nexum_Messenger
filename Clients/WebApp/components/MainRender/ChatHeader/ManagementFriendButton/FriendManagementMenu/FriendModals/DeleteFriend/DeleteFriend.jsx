import { useState, useEffect } from 'react';
import styles from '../Modal.module.css';
import { api } from '../../../../../../../services/api.js';

export default function DeleteFriend({ onClose, isOpen, selectedItem }) {
    const [error, setError] = useState('');
    const [modalState, setModalState] = useState('');
    const [isDeleting, setIsDeleting] = useState(false);

    useEffect(() => {
        if (isOpen) {
            setModalState('open');
            setError('');
        } else if (modalState === 'open') {
            setModalState('closing');
            const timer = setTimeout(() => setModalState(''), 300);
            return () => clearTimeout(timer);
        }
    }, [isOpen, modalState]);

    const handleDelete = async () => {
        setIsDeleting(true);
        setError('');

        try {
            const response = await api.delete(`/friends/remove/${selectedItem.id}`);

            if (response.data.success) {
                if (onClose) onClose();
            } else {
                setError(response.data.message || 'Ошибка при удалении пользователя из друзей');
            }
        }
        catch (error) {
            console.error('Error when deleting a user from friends:', error);
            setError(error.response?.data?.message ||
                error.response?.data?.errors?.join(', ') ||
                    'Ошибка при удалении пользователя из друзей');
        } finally {
            setIsDeleting(false);
        }
    };

    if (!isOpen && modalState === '') return null;

    return (
        <div className={`${styles.modalOverlay} ${modalState ? styles[modalState] : ''}`}>
                <div className={styles.modalContent}>
                    <button className={styles.closeButton} onClick={onClose}>&times;</button>
                    <h2 className={styles.headerText}>Удаление из друзей</h2>
    
                    {error && <div className={styles.error}>{error}</div>}
    
                    <div className={styles.formInput}>
                        <p style={{ color: '#fff', marginBottom: '20px', textAlign: 'center' }}>
                            Вы уверены, что хотите удалить данного пользователя из друзей?
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
                                onClick={handleDelete}
                                disabled={isDeleting}
                                style={{ background: '#e74c3c' }}
                            >
                                {isDeleting ? 'Удаление...' : 'Удалить из друзей.'}
                            </button>
                        </div>
                    </div>
                </div>
        </div>
    );
}