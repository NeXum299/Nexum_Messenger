using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServerMessenger.Application.DTO;
using ServerMessenger.Application.Results;

namespace ServerMessenger.Application.Interface.Services
{
    /// <summary>Интерфейс для работы с группами.</summary>
    public interface IGroupService
    {
        /// <summary>Создаёт новую группу с указанным названием и описанием.</summary>
        /// <param name="groupDto">DTO создаваемой группы.</param>
        /// <param name="creatorId">Идентификатолр создателя группы.</param>
        /// <returns>При успехе, возвращает успешный результат с созданной группой.</returns>
        /// <returns>При ошибке, возвращает проваленный результат и null с описанием ошибки.</returns>
        Task<Result<GroupDto>> CreateGroupAsync(GroupDto groupDto, Guid creatorId);

        /// <summary>Получает найденую группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Возвращает найденую группу.</returns>
        Task<Result<GroupDto>> GetGroupByIdAsync(Guid groupId);

        /// <summary>Получает список всех групп, в которых есть участник.</summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Возвращает все найденные группы.</returns>
        Task<Result<List<GroupDto>>> GetAllGroupByGroupMemberId(Guid userId);

        /// <summary>Обновляет группу.</summary>
        /// <param name="groupDto">DTO группы.</param>
        /// <returns>Обновлённый DTO группы.</returns>
        Task<Result<GroupDto>> UpdateGroupAsync(GroupDto groupDto);

        /// <summary>Удаляет группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Результат операции.</returns>
        Task<Result> RemoveGroupAsync(Guid groupId);
    }
}
