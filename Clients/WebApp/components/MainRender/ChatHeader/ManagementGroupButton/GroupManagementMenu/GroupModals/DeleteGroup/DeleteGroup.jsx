import { useState, useEffect } from 'react';
import styles from '../Modal.module.css';
import { api } from '../../../../../../../services/api.js';

export default function DeleteGroup({ onClose, isOpen, selectedItem }) {
    const [error, setError] = useState('');
    const [modalState, setModalState] = useState('');
    const [isDeleting, setIsDeleting] = useState(false);
    const [confirmationText, setConfirmationText] = useState('');

    useEffect(() => {
        if (isOpen) {
            setModalState('open');
            setConfirmationText('');
            setError('');
        } else if (modalState === 'open') {
            setModalState('closing');
            const timer = setTimeout(() => setModalState(''), 300);
            return () => clearTimeout(timer);
        }
    }, [isOpen, modalState]);

    const handleDelete = async () => {
        if (confirmationText !== selectedItem.name) {
            setError(`Пожалуйста, введите "${selectedItem.name}" для подтверждения`);
            return;
        }

        setIsDeleting(true);
        setError('');

        try {
            const response = await api.delete(`/groups/${selectedItem.id}`);
            
            if (response.data.success) {
                if (onClose) onClose();
                // Можно добавить callback для обновления состояния после удаления
            } else {
                setError(response.data.message || 'Ошибка при удалении группы');
            }
        }
        catch (error) {
            console.error('Error deleting group:', error);
            setError(
                error.response?.data?.message ||
                error.response?.data?.errors?.join(', ') ||
                'Ошибка при удалении группы'
            );
        } finally {
            setIsDeleting(false);
        }
    };

    const handleInputChange = (e) => {
        setConfirmationText(e.target.value);
        if (error) setError('');
    };

    if (!isOpen && modalState === '') return null;

    return (
        <div className={`${styles.modalOverlay} ${modalState ? styles[modalState] : ''}`}>
            <div className={styles.modalContent} style={{ maxWidth: '500px' }}>
                <button className={styles.closeButton} onClick={onClose}>&times;</button>
                <h2 className={styles.headerText}>Удалить группу</h2>

                {error && <div className={styles.error}>{error}</div>}

                <div className={styles.formInput}>
                    <p style={{ color: '#fff', marginBottom: '20px', textAlign: 'center' }}>
                        Вы собираетесь удалить группу <strong style={{ color: '#e74c3c' }}>"{selectedItem?.name}"</strong>.
                        Это действие нельзя отменить. Все данные группы будут безвозвратно удалены.
                    </p>

                    <p style={{ color: '#fff', marginBottom: '15px', textAlign: 'center' }}>
                        Для подтверждения введите название группы:
                    </p>

                    <input
                        type="text"
                        className={styles.inputText}
                        placeholder={selectedItem ? `Введите "${selectedItem.name}"` : 'Название группы'}
                        value={confirmationText}
                        onChange={handleInputChange}
                        disabled={isDeleting || !selectedItem}
                    />
                    
                    <div style={{ display: 'flex', gap: '10px', justifyContent: 'center', marginTop: '20px' }}>
                        <button 
                            className={styles.modalButton} 
                            onClick={onClose}
                            disabled={isDeleting}
                            style={{ background: '#666', flex: 1 }}
                        >
                            Отмена
                        </button>
                        
                        <button 
                            className={styles.modalButton} 
                            onClick={handleDelete}
                            disabled={isDeleting || confirmationText !== selectedItem?.name || !selectedItem}
                            style={{ 
                                background: confirmationText === selectedItem?.name ? '#e74c3c' : '#666',
                                opacity: confirmationText === selectedItem?.name ? 1 : 0.6,
                                flex: 1
                            }}
                        >
                            {isDeleting ? 'Удаление...' : 'Удалить группу'}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
