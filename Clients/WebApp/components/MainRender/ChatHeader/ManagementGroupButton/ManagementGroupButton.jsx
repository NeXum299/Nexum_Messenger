import styles from './ManagementGroupButton.module.css';

export default function ManagementGroupButton({ onClick }) {
    return (
        <button 
            className={styles["header-button"]} 
            onClick={onClick}
            aria-label="Управление группой"
        >⋮</button>
    );
}
