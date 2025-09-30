import styles from '../MessageInput.module.css';
import IconVoice from '../../../../src/assets/images/voice-button.svg?react';

export default function VoiceButton() {
  return (
      <button className={styles["buttons-input"]}>
          <IconVoice className={styles["input-buttons-image"]} />
      </button>
  );
}
