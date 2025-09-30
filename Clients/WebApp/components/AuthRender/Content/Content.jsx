import styles from './Content.module.css';

export default function Content({ onOpenLogin, onOpenRegistration }) {
    return (
        <main className={styles["content-container"]}>
            <div className={styles["content"]}>
                <div className={styles["text-section"]}>
                    <p>Добро пожаловать! Войдите в свой аккаунт или
                        зарегистрируйтесь, чтобы начать общение.</p>
                </div>

                <div className={styles["buttons-section"]}>
                    <button onClick={onOpenLogin}>Войти</button>
                    <button onClick={onOpenRegistration}>Зарегистрироваться</button>
                </div>
            </div>
        </main>
    );
}
