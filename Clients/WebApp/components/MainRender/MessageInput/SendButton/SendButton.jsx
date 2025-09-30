import styles from '../MessageInput.module.css';
import IconSend from '../../../../src/assets/images/send-button.svg?react';

export default function SendButton({ handleSendMessage }) {
    return (
        <button
            className={styles["buttons-input"]}
            onClick={handleSendMessage}
        >
            <IconSend className={styles["input-buttons-image"]} />
        </button>
    );
}
