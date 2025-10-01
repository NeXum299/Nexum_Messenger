using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Application.DTO;
using Server.Core.Entities;

namespace Server.Application.Interface.Repositories
{
    /// <summary>Интерфейс репозитория для работы с группой.</summary>
    public interface IGroupRepository
    {
        /// <summary>Создаёт новую группу с указанным названием и описанием.</summary>
        /// <param name="group">Группа.</param>
        /// <param name="creator">Создатель группы.</param>
        /// <returns>При успехе, возвращает успешный результат с созданной группой.</returns>
        /// <returns>При ошибке, возвращает проваленный результат и null с описанием ошибки.</returns>
        Task<GroupEntity> CreateGroupAsync(GroupEntity group, UserEntity creator);

        /// <summary>Получает найденую группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Возвращает найденую группу.</returns>
        Task<GroupEntity> GetGroupByIdAsync(Guid groupId);

        /// <summary>Получает список всех групп, в которых есть участник.</summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Возвращает все найденные группы.</returns>
        Task<List<GroupEntity>> GetAllGroupByUserId(Guid userId);

        /// <summary>Обновляет данные об группе.</summary>
        /// <param name="groupDto">DTO группы.</param>
        /// <returns>Обновлённую группу.</returns>
        Task<GroupEntity> UpdateGroupAsync(GroupDto groupDto);

        /// <summary>Удаляет группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Результат операции.</returns>
        Task DeleteGroupAsync(Guid groupId);
    }
}
