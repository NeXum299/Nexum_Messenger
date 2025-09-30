import { StrictMode, useState, useEffect } from 'react';
import AuthRender from '../AuthRender/AuthRender.jsx';
import MainRender from '../MainRender/MainRender.jsx';
import { api } from '../../services/api.js';

// Корневой компонент, управляющий состоянием аутентификации
export default function RenderPage() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const checkAuth = async () => {
            try {
                const response = await api.get('/auth/check');
                if (response.data?.isAuthenticated) {
                    setIsAuthenticated(true);
                } else {
                    setIsAuthenticated(false);
                }
            }
            catch (error) {
                // 401 - это нормально для неаутентифицированного пользователя
                if (error.response?.status === 401) {
                    setIsAuthenticated(false);
                } else {
                    console.error('Auth check error:', error);
                }
            }
            finally {
                setIsLoading(false);
            }
        };

        checkAuth();
    }, []);

    if (isLoading) {
        return <div>Loading...</div>;
    }

    const handleLogout = async () => {
        try {
            await api.post('/api/auth/logout', {}, { withCredentials: true });
        }
        finally {
            setIsAuthenticated(false);
        }
    };

    return (
        <StrictMode>
            {isAuthenticated ? (
                <MainRender onLogout={handleLogout} />
            ) : (
                <AuthRender onLogin={() => setIsAuthenticated(true)} />
            )}
        </StrictMode>
    );
}
