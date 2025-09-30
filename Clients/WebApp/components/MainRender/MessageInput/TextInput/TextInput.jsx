import styles from './TextInput.module.css';

export default function TextInput({ message, onKeyDown, setMessage }) {
    return (
        <input
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            className={styles["text-input"]}
            type="text"
            placeholder="Напишите сообщение..."
            onKeyDown={onKeyDown} />
    );
}
