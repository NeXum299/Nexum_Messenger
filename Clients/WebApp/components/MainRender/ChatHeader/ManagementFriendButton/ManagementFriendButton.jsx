import styles from './ManagementFriendButton.module.css';

export default function ManagementFriendButton({ onClick }) {
    return (
        <button 
            className={styles["header-button"]} 
            onClick={onClick}
            aria-label="Управление чатом"
        >⋮</button>
    );
}
