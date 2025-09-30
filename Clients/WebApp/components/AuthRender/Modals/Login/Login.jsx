import { useState, useEffect } from 'react';
import { api } from '../../../../services/api.js';
import styles from '../Modal.module.css';

export default function Login({ onClose, onLogin, isOpen }) {
    const [formData, setFormData] = useState({
        phoneNumber: '',
        password: ''
    });

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
            await api.post('/auth/login', formData);

            if (onLogin) onLogin();
            if (onClose) onClose();
        }
        catch (error) {
            console.error('Login error:', error);
            setError(error.response?.data?.message || 'Ошибка входа. Проверьте данные.');
        }
    };

    if (!isOpen && modalState === '') return null;

    return (
        <div className={`${styles.modalOverlay} ${modalState ? styles[modalState] : ''}`}>
            <div className={styles.modalContent}>
                <button className={styles.closeButton} onClick={onClose}>&times;</button>

                <h2 className={styles.headerText}>Вход</h2>

                {error && <div className={styles.error}>{error}</div>}

                <form className={styles.formInput}>
                    <div>
                        <input
                            className={styles.inputText}
                            type="text"
                            onChange={handleChange}
                            name="phoneNumber"
                            value={formData.phoneNumber}
                            placeholder="Номер телефона"
                            required />
                    </div>
                    <div>
                        <input
                            className={styles.inputText}
                            type="password"
                            onChange={handleChange}
                            name="password"
                            value={formData.password}
                            placeholder="Пароль"
                            required />
                    </div>

                    <button className={styles.modalButton} type="submit" onClick={handleSubmit}>Войти</button>
                </form>
            </div>
        </div>
    );
}
