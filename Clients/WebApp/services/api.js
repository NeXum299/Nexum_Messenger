import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:5001/api',
    headers: {
        'Content-Type': 'application/json'
    },
    withCredentials: true
});

// Интерцептор для обработки 401 ошибки и обновления токена
api.interceptors.response.use(response => response, async error => {
    const originalRequest = error.config;

    // Не пытаемся обновлять токен, если это запрос на /auth/check
    if (error.response?.status === 401 && !originalRequest._retry &&
        !originalRequest.url.includes('auth/check')) {
        originalRequest._retry = true;

        try {
            await axios.post(
                'https://localhost:5001/api/token/refresh',
                {},
                { withCredentials: true }
            );

            return api(originalRequest);
        }
        catch (refreshError) {
            window.location.href = '/';
            return Promise.reject(refreshError);
        }
    }

    return Promise.reject(error);
});

export { api };
