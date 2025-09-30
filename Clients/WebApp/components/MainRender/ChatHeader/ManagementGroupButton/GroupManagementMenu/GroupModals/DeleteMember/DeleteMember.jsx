import { useState, useEffect } from 'react';
import styles from '../Modal.module.css';
import { api } from '../../../../../../../services/api.js';

export default function DeleteMember({ onClose, isOpen, selectedItem }) {
    const [formData, setFormData] = useState({username: ''});
    const [error, setError] = useState('');
    const [modalState, setModalState] = useState('');

    useEffect(() => {
        if (isOpen) {
            setModalState('open');
        } else if (modalState === 'open') {
            setModalState('closing');
            const timer = setTimeout(() => setModalState(''), 300);
            return () => clearTimeout(timer);
        }
    }, [isOpen, modalState]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        try {
            const encodedUsername = encodeURIComponent(formData.username.trim());
            await api.delete(`/group_members/${selectedItem.id}/members/${encodedUsername}`);
            if (onClose) onClose();
        }
        catch (error) {       
            if (error.response?.data?.errors) {
                console.error('Server errors:', error.response.data.errors);
            }
            
            setError(error.response?.data?.message || 
                    error.response?.data?.errors?.join(', ') || 
                    'Ошибка добавления пользователя в группу');
        }
    };

    if (!isOpen && modalState === '') return null;

    return (
            <div className={`${styles.modalOverlay} ${modalState ? styles[modalState] : ''}`}>
                <div className={styles.modalContent}>
                    <button className={styles.closeButton} onClick={onClose}>&times;</button>
                    <h2 className={styles.headerText}>Удаление участника</h2>
    
                    {error && <div className={styles.error}>{error}</div>}
    
                    <form className={styles.formInput}>
                        <div>
                            <input
                                className={styles.inputText}
                                onChange={handleChange}
                                type="text"
                                name="username"
                                value={formData.username}
                                placeholder="Ник пользователя..."
                                required />
                        </div>
    
                        <button className={styles.modalButton} type="submit" onClick={handleSubmit}>Удалить</button>
                    </form>
                </div>
            </div>
    );
}
