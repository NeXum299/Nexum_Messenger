import { useState, useEffect } from 'react';
import { api } from '../../../services/api.js'
import styles from './ItemList.module.css';

export default function ItemList({ onSelect, refreshTrigger }) {
    const [items, setItems] = useState([]);
    const [error, setError] = useState(null);
    
    useEffect(() => {
        const fetchData = async () => {
            try {
                // Загрузка групп
                const groupsResponse = await api.get('/groups');
                const groups = groupsResponse.data?.success && Array.isArray(groupsResponse.data.value) 
                    ? groupsResponse.data.value.map(group => ({ ...group, type: 'group' }))
                    : [];

                // Загрузка друзей
                const friendsResponse = await api.get('/friends');
                const friends = friendsResponse.data?.success && Array.isArray(friendsResponse.data.value) 
                    ? friendsResponse.data.value.map(friend => ({ ...friend, type: 'friend' }))
                    : [];


                // Объединяем и сортируем по имени
                const allItems = [...groups, ...friends].sort((a, b) => {
                    const nameA = a.type === 'group' ? a.name : `${a.firstName} ${a.lastName}`;
                    const nameB = b.type === 'group' ? b.name : `${b.firstName} ${b.lastName}`;
                    return nameA.localeCompare(nameB);
                });

                setItems(allItems);
            } catch (error) {
                console.error('Ошибка при загрузке данных:', error);
                setError(error.message);
            }
        };

        fetchData();
    }, [refreshTrigger]);
    
    if (error) return <div>Ошибка: {error}</div>;

return (
        <div className={styles["items-container"]}>
            <p className={styles["header-text"]}>Друзья и группы</p>

            <div className={styles["items-list"]}>
                {items.map(item => (
                    <div
                        key={item.id}
                        className={item.type === 'group' ? styles["group-item"] : styles["friend-item"]}
                        onClick={() => onSelect && onSelect(item)}
                    >
                        <div className={styles["item-content"]}>
                            <img
                                className={item.type === 'group' ? styles["group-avatar"] : styles["friend-avatar"]}
                                src={`https://localhost:5001${item.avatarPath}`}
                                alt={item.type === 'group' ? item.name : item.userName}
                                onError={(e) => {
                                    e.target.src = item.type === 'group' 
                                        ? 'https://localhost:5001/avatars/groups/default.jpg'
                                        : 'https://localhost:5001/avatars/users/default.jpg';
                                }}
                            />

                            <div className={styles["item-info"]}>
                                {item.type === 'group' ? (
                                    <p className={styles["group-name"]}>{item.name}</p>
                                ) : (
                                    <>
                                        <p className={styles["friend-name"]}>
                                            {item.firstName} {item.lastName}
                                        </p>
                                        <p className={styles["friend-username"]}>@{item.userName}</p>
                                    </>
                                )}
                            </div>

                            {/* Индикатор типа */}
                            <span className={styles["item-type-badge"]}>
                                {item.type === 'group' ? 'Группа' : 'Друг'}
                            </span>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}
