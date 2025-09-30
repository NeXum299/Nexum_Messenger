import { useRef, useState } from 'react';
import styles from './FriendManagementMenu.module.css';
import DeleteFriend from './FriendModals/DeleteFriend/DeleteFriend.jsx';

export default function FriendManagementMenu({ isOpen, onClose, selectedItem, onCloseItem }) {
    const menuRef = useRef(null);
    const [activeModal, setActiveModal] = useState(null);

    const handleDeleteFriendClick = () => {
        setActiveModal('deleteFriend');
    }

    const handleModalClose = () => {
        setActiveModal(null);
        onClose();
    };

    const handleCloseChat = () => {
        onCloseItem();
        onClose();
    };

    if (!isOpen) return null;

    return (
        <>
            <div className={styles["menu-overlay"]}>
                <div 
                    ref={menuRef} 
                    className={`${styles["menu-container"]} ${isOpen ? styles["menu-open"] : ""}`}
                >
                    <div className={styles["menu-content"]}>
                        <button
                            className={styles["menu-item"]}
                            onClick={handleCloseChat}>
                            Закрыть чат
                        </button>

                        <button
                            className={styles["menu-item"]}
                            onClick={handleDeleteFriendClick}>
                            Удалить из друзей
                        </button>
                    </div>
                </div>
            </div>

            {activeModal === 'deleteFriend' && (
                <DeleteFriend
                    isOpen={true}
                    onClose={handleModalClose}
                    selectedItem={selectedItem} />
            )}
        </>
    );
}
