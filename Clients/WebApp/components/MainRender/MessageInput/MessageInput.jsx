import TextInput from './TextInput/TextInput.jsx';
import VoiceButton from './VoiceButton/VoiceButton.jsx';
import SendButton from './SendButton/SendButton.jsx';
import styles from './MessageInput.module.css';
import { api } from '../../../services/api.js';

export default function MessageInput({ selectedFriend, selectedGroup }) {
    const [message, setMessage] = useState('');
    
    const handleSendMessage = async () => {
        if (!message.trim()) return;

        const selectedItem = selectedFriend || selectedGroup;
        if (!selectedItem) return;

        try {
            if (selectedItem.type === 'group') {
                await api.post(`/groups/${selectedItem.id}/messages`, message);
            } else {
                // Для личных сообщений
                console.log('Sending private message:', message);
            }

            setMessage('');
        } catch (error) {
            console.error('Error sending message:', error);
        }
    };

    const onKeyDown = (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            handleSendMessage();
        }
    };

    if (!selectedFriend && !selectedGroup) {
    return null;
  }

    return (
        <>
            <TextInput
                onKeyDown={onKeyDown}
                message={message}
                setMessage={setMessage} />

            <div className={styles["buttons-container"]}>
                <SendButton
                    handleSendMessage={handleSendMessage}
                    disabled={!message.trim()} />
                <VoiceButton />
            </div>
        </>
    );
}
