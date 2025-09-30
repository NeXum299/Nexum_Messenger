import { useState, useRef, useEffect } from 'react';
import { api } from '../../../../services/api.js';
import styles from './CreateGroup.module.css';

export default function CreateGroup({ onClose, onCreateSuccess }) {
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [error, setError] = useState('');
    const [isClosing, setIsClosing] = useState(false);
    const modalRef = useRef(null);

    useEffect(() => {
        document.body.style.overflow = 'hidden';

        const handleClickOutside = (event) => {
            if (modalRef.current && !modalRef.current.contains(event.target)) {
                handleClose();
            }
        };

        const handleEscape = (event) => {
            if (event.key === 'Escape') {
                handleClose();
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        document.addEventListener('keydown', handleEscape);
        
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
            document.removeEventListener('keydown', handleEscape);
            document.body.style.overflow = 'unset';
        };
    }, [onClose]);

    const handleClose = () => {
        setIsClosing(true);
        setTimeout(() => {
            onClose();
        }, 300);
    };

    const handleSubmit = async (e) =>
    {
        e.preventDefault();
        try {
            const response = await api.post('/groups', {
                name,
                description
            });

            if (response.status === 201 && response.data?.success) {
                onClose();
                if (onCreateSuccess) {
                    onCreateSuccess(response.data.value);
                } else {
                    setError('Неизвестная ошибка при создании группы');
                }
            }
        } catch (err) {
            setError(err.response?.data?.errors || 'Ошибка при создании группы');
        }
    };

    return (
        <div className={`${styles["modal-overlay"]} ${isClosing ? styles["closing"] : ""}`}>
            <div 
                ref={modalRef}
                className={`${styles["modal-content"]} ${isClosing ? styles["closing"] : ""}`}>
                
                <div className={styles["modal-header"]}>
                    <h2 className={styles["header-text"]}>Создать группу</h2>
                    <button onClick={handleClose} className={styles["close-button"]}>&times;</button>
                </div>

                <form onSubmit={handleSubmit} className={styles.form}>
                    <div className={styles.inputContainer}>
                        <input
                            type="text"
                            placeholder="Название группы"
                            required
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            className={styles.input}
                        />
                    </div>

                    <div className={styles.inputContainer}>
                        <textarea
                            placeholder="Описание группы (необязательно)"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            className={styles.textarea}
                            rows="3"
                        />
                    </div>

                    {error && <div className={styles.error}>{error}</div>}

                    <button type="submit" className={styles.submitButton}>
                        Создать группу
                    </button>
                </form>
            </div>
        </div>
    );
}
