import { useState, useEffect } from 'react';
import { api } from '../../../../services/api.js';

export default function FriendRequests({ onClose }) {
    const [requests, setRequests] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        fetchRequests();
    }, []);

    const fetchRequests = async () => {
        try {
            const response = await api.get('/friends/incoming');
            if (response.data?.success) {
                setRequests(response.data.value || []);
            }
        } catch (err) {
            setError('Ошибка при загрузке запросов');
        } finally {
            setLoading(false);
        }
    };

    const handleAccept = async (userId) => {
        try {
            const response = await api.post(`/friends/accept/${userId}`);
            if (response.data?.success) {
                // Обновляем список запросов
                setRequests(requests.filter(req => req.userId !== userId));
            }
        } catch (err) {
            setError('Ошибка при принятии запроса');
        }
    };

    const handleReject = async (userId) => {
        try {
            const response = await api.delete(`/friends/remove/${userId}`);
            if (response.data?.success) {
                // Обновляем список запросов
                setRequests(requests.filter(req => req.userId !== userId));
            }
        } catch (err) {
            setError('Ошибка при отклонении запроса');
        }
    };

    if (loading) return <div>Загрузка...</div>;

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <button onClick={onClose}>Закрыть</button>
                <h2>Входящие запросы в друзья</h2>
                
                {error && <div className="error">{error}</div>}
                
                {requests.length === 0 ? (
                    <p>Нет входящих запросов</p>
                ) : (
                    <div className="requests-list">
                        {requests.map(request => (
                            <div key={request.id} className="request-item">
                                <div className="request-user-info">
                                    <img
                                        src={`https://localhost:5001${request.avatarPath}`}
                                        alt={request.userName}
                                        className="request-avatar"
                                        onError={(e) => {
                                            e.target.src = 'https://localhost:5001/avatars/users/default.jpg';
                                        }}
                                    />
                                    <div>
                                        <p className="request-name">
                                            {request.firstName} {request.lastName}
                                        </p>
                                        <p className="request-username">@{request.userName}</p>
                                    </div>
                                </div>
                                
                                <div className="request-actions">
                                    <button 
                                        onClick={() => handleAccept(request.userId)}
                                        className="accept-btn"
                                    >
                                        Принять
                                    </button>
                                    <button 
                                        onClick={() => handleReject(request.userId)}
                                        className="reject-btn"
                                    >
                                        Отклонить
                                    </button>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}
