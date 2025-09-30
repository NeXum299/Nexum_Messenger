import { useState, useEffect } from 'react';
import styles from '../Modal.module.css';
import { api } from '../../../../../../../services/api.js';

export default function EditGroup({ onClose, isOpen, selectedItem }) {
    const [formData, setFormData] = useState({
        id: '',
        name: '',
        description: ''
    });

    const [error, setError] = useState('');
    const [modalState, setModalState] = useState('');

    useEffect(() => {
        if (selectedItem) {
            setFormData({
                id: selectedItem.id,
                name: selectedItem.name || '',
                description: selectedItem.description || ''
            });
        }
        
        if (isOpen) {
            setModalState('open');
        } else if (modalState === 'open') {
            setModalState('closing');
            const timer = setTimeout(() => setModalState(''), 300);
            return () => clearTimeout(timer);
        }
    }, [isOpen, modalState, selectedItem]);

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
            await api.put('/groups', formData);
            if (onClose) onClose();
        }
        catch (error) {
            const errorData = error.response?.data;
            
            if (errorData?.errors) {
                if (Array.isArray(errorData.errors)) {
                    setError(errorData.errors.join(', '));
                } else if (typeof errorData.errors === 'string') {
                    setError(errorData.errors);
                } else {
                    setError('Неизвестный формат ошибки');
                }
            } else if (errorData?.message) {
                setError(errorData.message);
            } else {
                setError('Ошибка изменения данных в группе');
            }
        }
    };

    if (!isOpen && modalState === '') return null;

    return (
        <div className={`${styles.modalOverlay} ${modalState ? styles[modalState] : ''}`}>
            <div className={styles.modalContent}>
                <button className={styles.closeButton} onClick={onClose}>&times;</button>
                <h2 className={styles.headerText}>Изменение группы</h2>

                {error && <div className={styles.error}>{error}</div>}

                <form className={styles.formInput} onSubmit={handleSubmit}>
                    <div>
                        <input
                            className={styles.inputText}
                            onChange={handleChange}
                            type="text"
                            name="name"
                            value={formData.name}
                            placeholder="Название группы..."
                            required
                        />
                    </div>
                    
                    <div>
                        <textarea
                            className={styles.inputText}
                            onChange={handleChange}
                            name="description"
                            value={formData.description}
                            placeholder="Описание группы..."
                            rows="4"
                            required
                        />
                    </div>

                    <button className={styles.modalButton} type="submit">Сохранить изменения</button>
                </form>
            </div>
        </div>
    );
}
