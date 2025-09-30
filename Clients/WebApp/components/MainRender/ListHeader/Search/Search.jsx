import styles from './Search.module.css';

export default function Search() {
  return (
      <input className={styles["search-input"]} type="search" placeholder="Поиск..." />
  );
}
