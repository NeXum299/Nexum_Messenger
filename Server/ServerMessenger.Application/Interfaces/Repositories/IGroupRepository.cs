using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServerMessenger.Application.DTO;
using ServerMessenger.Application.Results;
using ServerMessenger.Core.Entities;

namespace ServerMessenger.Application.Interface.Repositories
{
    /// <summary>Интерфейс репозитория для работы с группой.</summary>
    public interface IGroupRepository
    {
        /// <summary>Создаёт новую группу с указанным названием и описанием.</summary>
        /// <param name="group">Группа.</param>
        /// <param name="creator">Создатель группы.</param>
        /// <returns>При успехе, возвращает успешный результат с созданной группой.</returns>
        /// <returns>При ошибке, возвращает проваленный результат и null с описанием ошибки.</returns>
        Task<Result<Group>> CreateGroupAsync(Group group, User creator);

        /// <summary>Получает найденую группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Возвращает найденую группу.</returns>
        Task<Result<Group>> GetGroupByIdAsync(Guid groupId);

        /// <summary>Получает список всех групп, в которых есть участник.</summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Возвращает все найденные группы.</returns>
        Task<Result<List<Group>>> GetAllGroupByUserId(Guid userId);

        /// <summary>Обновляет данные об группе.</summary>
        /// <param name="groupDto">DTO группы.</param>
        /// <returns>Обновлённую группу.</returns>
        Task<Result<Group>> UpdateGroupAsync(GroupDto groupDto);

        /// <summary>Удаляет группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Результат операции.</returns>
        Task<Result> DeleteGroupAsync(Guid groupId);
    }
}
