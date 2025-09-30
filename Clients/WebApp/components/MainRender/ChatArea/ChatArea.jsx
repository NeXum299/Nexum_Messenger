import { useState, useEffect, useRef } from 'react';
import styles from './ChatArea.module.css';
import { signalRService } from '../../../services/signalRService';
import { api } from '../../../services/api.js';

export default function ChatArea({ selectedItem }) {
    const [messages, setMessages] = useState([]);
    const [loading, setLoading] = useState(false);
    const messagesEndRef = useRef(null);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages]);
  
    useEffect(() => {
        const fetchMessages = async () => {
            if (!selectedItem) return;

            setLoading(true);
            try {
                let response;

                if (selectedItem.type === 'group') {
                    response = await api.get(`/groups/${selectedItem.id}/messages?limit=100`)
                } else {
                    //response = await api.get(`/users/${selectedFriend.id}/messages?limit=100`);
                }

                setMessages(response.data);
            } catch (error) {
                console.error('Error fetching messages:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchMessages();
    }, [selectedItem]);

    useEffect(() => {
        if (!selectedItem) return;

        const connectToGroup = async () => {
            await signalRService.startConnection();
            await signalRService.joinGroup(selectedItem.id);
            
            signalRService.onReceiveMessage((message) => {
                setMessages(prev => [...prev, message]);
            });
        };

        connectToGroup();

        return () => {
            if (selectedItem) {
                signalRService.leaveGroup(selectedItem.id);
            }
        };
    }, [selectedItem]);

    if (!selectedItem) {
        return (
            <div className={styles["chat-area"]}>
                <div className={styles["chat-content"]}>
                    <div className={styles["no-chat-selected"]}>
                        Выберите чат для начала общения
                    </div>
                </div>
            </div>
        );
    }

    if (loading) {
        return (
            <div className={styles["chat-area"]}>
                <div className={styles["loading"]}>Загрузка сообщений...</div>
            </div>
        );
    }

    return (
        <div className={styles["chat-area"]}>
            <div className={styles["chat-content"]}>
                {messages.map((message) => (
                    <div key={message.messageId || message.id} className={styles["message"]}>
                        <div className={styles["message-header"]}>
                            <span className={styles["sender"]}>
                                {message.senderId === localStorage.getItem('userId') ? 'Вы' : message.senderName}
                            </span>
                            <span className={styles["time"]}>
                                {new Date(message.sentAt).toLocaleTimeString()}
                            </span>
                        </div>
                        <div className={styles["message-content"]}>
                            {message.content}
                        </div>
                        {message.isEdited && (
                            <span className={styles["edited"]}>(ред.)</span>
                        )}
                    </div>
                ))}
            </div>
        </div>
    );
}
