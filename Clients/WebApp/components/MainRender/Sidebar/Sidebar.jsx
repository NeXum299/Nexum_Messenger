import { useState, useEffect } from 'react';
import IconPolicy from '../../../src/assets/images/policy-button.svg?react';
import IconAddFriend from '../../../src/assets/images/add-friend.svg?react';
import IconCreateChannel from '../../../src/assets/images/create-channel.svg?react';
import IconCreateGroup from '../../../src/assets/images/create-group.svg?react';
import IconCustomization from '../../../src/assets/images/customization-button.svg?react';
import IconLanguageSettings from '../../../src/assets/images/language-settings.svg?react';
import IconNightMode from '../../../src/assets/images/night-mode.svg?react';
import IconMyProfile from '../../../src/assets/images/my-profile.svg?react';
//import AddFriend from '../../../src/assets/images/add-friend.svg?react';
//import IconFriendRequests from '../../../src/assets/images/friend-requests.svg?react';
import styles from './Sidebar.module.css';
import { api } from '../../../services/api.js';

export default function Sidebar({
    onMyProfileOpen, 
    onCustomizationOpen,
    onCreateGroupOpen, 
    onCreateChannelOpen, 
    onPolicyOpen,
    onAddFriendOpen, 
    onFriendRequestsOpen}) {
    const [unreadRequestsCount, setUnreadRequestsCount] = useState(0);
    
    useEffect(() => {
        const fetchUnreadCount = async () => {
            try {
                const response = await api.get('/friends/incoming');
                if (response.data?.success) {
                    setUnreadRequestsCount(response.data.value?.length || 0);
                }
            } catch (error) {
                console.error('Error fetching unread requests count:', error);
            }
        };
        
        fetchUnreadCount();
    }, []);

    return (
        <>
            <div>
                 <button className={styles["buttons"]} onClick={onMyProfileOpen}>
                    <div className={styles["button-content"]}>
                        <IconMyProfile className={styles["buttons-image"]} />
                        <span className={styles["button-text"]}>Мой профиль</span>
                    </div>
                </button>
            </div>

            <div>
                <button
                    className={styles["buttons"]}
                    onClick={onAddFriendOpen}>
                    <div className={styles["button-content"]}>
                        <IconAddFriend className={styles["buttons-image"]} />
                        <span className={styles["button-text"]}>Друзья</span>
                    </div>
                </button>
            </div>

            <button 
                className={styles["buttons"]} 
                onClick={onFriendRequestsOpen}
                title="Запросы в друзья">
                    <div className={styles["button-content"]}>
                        <IconAddFriend className={styles["buttons-image"]} />
                        {unreadRequestsCount > 0 && (
                            <span className={styles["badge"]}>{unreadRequestsCount}</span>
                        )}
                        <span className={styles["button-text"]}>Заявки в друзья</span>
                    </div>
            </button>

            <div>
                <button className={styles["buttons"]} onClick={onCreateGroupOpen}>
                    <div className={styles["button-content"]}>
                        <IconCreateGroup className={styles["buttons-image"]} />
                        <span className={styles["button-text"]}>Создать группу</span>
                    </div>
                </button>
            </div>

            <div>
                <button className={styles["buttons"]} onClick={onCreateChannelOpen}>
                    <div className={styles["button-content"]}>
                        <IconCreateChannel className={styles["buttons-image"]} />
                        <span className={styles["button-text"]}>Создать канал</span>
                    </div>
                </button>
            </div>

            <div>
                <button className={styles["buttons"]} onClick={onCustomizationOpen}>
                    <div className={styles["button-content"]}>
                        <IconCustomization className={styles["buttons-image"]} />
                        <span className={styles["button-text"]}>Кастомизация</span>
                    </div>
                </button>
            </div>

            <div>
                <button className={styles["buttons"]} onClick={onPolicyOpen}>
                    <div className={styles["button-content"]}>
                        <IconPolicy className={styles["buttons-image"]} />
                        <span className={styles["button-text"]}>Приватность</span>
                    </div>
                </button>
            </div>

            <div>
                <button className={styles["buttons"]}>
                    <div className={styles["button-content"]}>
                        <IconLanguageSettings className={styles["buttons-image"]} />
                        <span className={styles["button-text"]}>Сменить язык</span>
                    </div>
                </button>
            </div>

            <div>
                <button className={styles["buttons"]}>
                    <div className={styles["button-content"]}>
                        <IconNightMode className={styles["buttons-image"]} />
                        <span className={styles["button-text"]}>Ночной режим</span>
                    </div>
                </button>
            </div>
        </>
    );
}
