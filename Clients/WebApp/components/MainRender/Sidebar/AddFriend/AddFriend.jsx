import { useState, useRef, useEffect } from 'react';
import { api } from '../../../../services/api.js';
import styles from './AddFriend.module.css';

export default function AddFriend({ onClose }) {
    const [username, setUsername] = useState('');
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
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

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        try {
            const response = await api.post('/friends/add', {
                userName: username
            });

            if (response.data?.success) {
                setSuccess('Запрос на добавление в друзья отправлен');
                setUsername('');
                setTimeout(() => {
                    handleClose();
                }, 2000);
            }
        } catch (err) {
            setError(err.response?.data?.errors || 'Ошибка при добавлении друга');
        }
    };

    return (
        <div className={`${styles["modal-overlay"]} ${isClosing ? styles["closing"] : ""}`}>
            <div 
                ref={modalRef}
                className={`${styles["modal-content"]} ${isClosing ? styles["closing"] : ""}`}>
                
                <div className={styles["modal-header"]}>
                    <h2 className={styles["header-text"]}>Добавить друга</h2>
                    <button onClick={handleClose} className={styles["close-button"]}>&times;</button>
                </div>

                <form onSubmit={handleSubmit} className={styles.form}>
                    <div className={styles.inputContainer}>
                        <input
                            type="text"
                            placeholder="Введите никнейм друга"
                            required
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            className={styles.input}
                        />
                    </div>

                    {error && <div className={styles.error}>{error}</div>}
                    {success && <div className={styles.success}>{success}</div>}

                    <button type="submit" className={styles.submitButton}>
                        Добавить в друзья
                    </button>
                </form>
            </div>
        </div>
    );
}
