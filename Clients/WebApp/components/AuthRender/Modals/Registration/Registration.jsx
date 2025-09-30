import { useState, useEffect } from 'react';
import { api } from '../../../../services/api.js';
import styles from '../Modal.module.css';

export default function Registration({ onClose, onLogin, isOpen }) {
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        userName: '',
        phoneNumber: '',
        birthDate: '',
        password: '',
        confirmPassword: '',
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

        if (formData.password !== formData.confirmPassword) {
            setError('Пароли не совпадают');
            return;
        }

        try {
            await api.post('/auth/register', formData);

            if (onLogin) onLogin();
            if (onClose) onClose();
        }
        catch (error) {
            console.error('Full registration error:', error);
            setError(error.response?.data?.message || error.response?.data?.errors?.join(', ') || 'Ошибка регистрации');
        }
    };

    if (!isOpen && modalState === '') return null;

    return (
        <div className={`${styles.modalOverlay} ${modalState ? styles[modalState] : ''}`}>
            <div className={styles.modalContent}>
                <button className={styles.closeButton} onClick={onClose}>&times;</button>
                <h2 className={styles.headerText}>Регистрация</h2>

                {error && <div className={styles.error}>{error}</div>}

                <form className={styles.formInput}>
                    <div className={styles.nameRow}>
                        <div>
                            <input
                                className={styles.inputText}
                                onChange={handleChange}
                                type="text"
                                name="firstName"
                                value={formData.firstName}
                                placeholder="Имя"
                                required />
                        </div>
                        <div>
                            <input
                                className={styles.inputText}
                                onChange={handleChange}
                                type="text"
                                name="lastName"
                                placeholder="Фамилия"
                                value={formData.lastName}
                                required />
                        </div>
                    </div>

                    <div>
                        <input
                            className={styles.inputText}
                            onChange={handleChange}
                            type="text"
                            name="userName"
                            placeholder="Никнейм. Например: (example8231)"
                            value={formData.userName} />
                    </div>

                    <div>
                        <input
                            className={styles.inputText}
                            onChange={handleChange}
                            type="phone"
                            name="phoneNumber"
                            placeholder="Номер телефона. Например: 0 (000)-000-00-00"
                            value={formData.phoneNumber} />
                    </div>

                    <div>
                        <input
                            className={styles.inputText}
                            onChange={handleChange}
                            type="date"
                            name="birthDate"
                            value={formData.birthDate} />
                    </div>

                    <div>
                        <input
                            className={styles.inputText}
                            onChange={handleChange}
                            type="password"
                            name="password"
                            value={formData.password}
                            placeholder="Пароль"
                            required />
                    </div>

                    <div>
                        <input
                            className={styles.inputText}
                            type="password"
                            name="confirmPassword"
                            onChange={handleChange}
                            value={formData.confirmPassword}
                            placeholder="Подтвердите пароль"
                            required />
                    </div>

                    <button className={styles.modalButton} type="submit" onClick={handleSubmit}>Зарегистрироваться</button>
                </form>
            </div>
        </div>
    );
}
