import styles from './ButtonsFilter.module.css';

export default function ButtonsFilter() {
    return (
        <>
            <div className={styles["people-filter-container"]}>
                <button className={styles["people-buttons-filter"]}>Каналы</button>
                <button className={styles["people-buttons-filter"]}>Группы</button>
                <button className={styles["people-buttons-filter"]}>Пользователи</button>
            </div>
            <div className={styles["message-filter-container"]}>
                <button className={styles["message-buttons-filter"]}>Прочитанные сообщения</button>
                <button className={styles["message-buttons-filter"]}>Непрочитанные сообщения</button>
            </div>
        </>
    );
}
