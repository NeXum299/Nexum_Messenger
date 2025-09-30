import { useState, useEffect } from 'react';
import { api } from '../services/api';

export const useGroupMembersCount = (groupId) => {
    const [count, setCount] = useState(0);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchMembersCount = async () => {
            if (!groupId) {
                setLoading(false);
                return;
            }

            try {
                setLoading(true);
                setError(null);
                
                const response = await api.get(`/group_members/${groupId}/members/count`);
                setCount(response.data.count);
            } catch (error) {
                console.error('Error fetching members count:', error);
                setError(error.response?.data?.message || 'Ошибка загрузки количества участников');
                setCount(0);
            } finally {
                setLoading(false);
            }
        };

        fetchMembersCount();
    }, [groupId]);

    const refreshCount = async () => {
        if (!groupId) return;
        
        try {
            setLoading(true);
            const response = await api.get(`/group_members/${groupId}/members/count`);
            setCount(response.data.count);
        } catch (error) {
            console.error('Error refreshing members count:', error);
            setError(error.response?.data?.message || 'Ошибка обновления количества участников');
        } finally {
            setLoading(false);
        }
    };

    return { count, loading, error, refreshCount };
};
