import styles from './Footer.module.css';

export default function Footer() {
    return (
        <footer className={styles.footer}>
            <div className={styles.footerContent}>
                <div className={styles.logoContainer}>
                    <img src="/images/logo.png" alt="Логотип компании" width="60"/>
                </div>

                <div className={styles.navLinks}>
                    <a>О нас</a>
                    <a>Блог</a>
                    <a>Контакты</a>
                </div>
            </div>

            <div className={styles.footerBottom}>
                <p>© 2024 Nexum. Все права защищены.</p>
                <nav className={styles.legalLinks}>
                    <a>Политика конфиденциальности</a>
                    <a>Условия использования</a>
                </nav>
            </div>
        </footer>
    );
}
