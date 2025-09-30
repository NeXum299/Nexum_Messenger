// ButtonMenu.jsx
import styles from './ButtonMenu.module.css';

export default function ButtonMenu({ onClick }) {
    return (
        <button 
            className={styles["menu-button"]}
            onClick={onClick}
            title="Меню">lll</button>
    );
}
