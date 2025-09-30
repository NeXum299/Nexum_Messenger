import { useState, useEffect, useRef } from 'react';
import { api } from '../../../../services/api.js';
import styles from './MyProfile.module.css';

export default function MyProfile({ onClose }) {
    const [avatar, setAvatar] = useState('https://localhost:5001/avatars/users/default.jpg');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [error, setError] = useState('');
    const [isClosing, setIsClosing] = useState(false);
    const modalRef = useRef(null);

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const [nameResponse, avatarResponse] = await Promise.all([
                    api.get('/users/name'),
                    api.get(`/users/avatar?t=${Date.now()}`)
                ]);

                if (nameResponse.data.success) {
                    setFirstName(nameResponse.data.firstName);
                    setLastName(nameResponse.data.lastName);
                }

                if (avatarResponse.data.success && avatarResponse.data.avatarPath) {
                    setAvatar(avatarResponse.data.avatarPath);
                }
            } catch (err) {
                console.error('Failed to fetch user data:', err);
                setError('Не удалось загрузить данные профиля');
            }
        };

        fetchUserData();
    }, []);

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

    const handleAvatarUpload = async (e) => {
        const file = e.target.files?.[0];
        if (!file) return;

        const formData = new FormData();
        formData.append('file', file);

        try {
            setError('');
            const response = await api.post('/users/update-avatar', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });

            if (response.data.success) {
                setAvatar(response.data.avatarPath);
            }
        } catch (err) {
            console.error('Avatar upload failed:', err);
            if (err.response?.data?.error) {
                setError(err.response.data.error);
            } else {
                setError('Не удалось загрузить аватар');
            }
        }
    };

    return (
        <div className={`${styles["modal-overlay"]} ${isClosing ? styles["closing"] : ""}`}>
            <div 
                ref={modalRef} 
                className={`${styles["modal-content"]} ${isClosing ? styles["closing"] : ""}`}
            >
                <div className={styles["modal-header"]}>
                    <h2 className={styles["header-text"]}>Мой профиль</h2>
                    <button onClick={handleClose} className={styles["close-button"]}>&times;</button>
                </div>

                <div className={styles["profile-info"]}>
                    <img
                        src={`${avatar}?t=${new Date().getTime()}`}
                        alt='Аватарка'
                        className={styles.avatar}
                        onError={(e) => {
                            e.target.src = 'https://localhost:5001/avatars/users/default.jpg';
                        }}
                    />

                    <div className={styles["name-container"]}>
                        <label>Имя: {firstName}</label>
                        <label>Фамилия: {lastName}</label>
                    </div>
                </div>

                <div className={styles["upload-section"]}>
                    <label htmlFor="file-upload" className={styles["upload-button"]}>
                        Загрузить аватар
                    </label>
                    <input
                        id="file-upload"
                        type="file"
                        accept="image/*"
                        style={{ display: 'none' }}
                        onChange={handleAvatarUpload} />
                    {error && <div className={styles["error-message"]}>{error}</div>}
                </div>
            </div>
        </div>
    );
}
