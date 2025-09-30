using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Application.DTO;
using Server.Application.Results;
using Server.Application.Roles;

namespace Server.Application.Interface.Services
{
    /// <summary>Интефрейс для работы с участниками одной группы.</summary>
    public interface IGroupMemberService
    {
        /// <summary>Добавляет пользователя в группу с указанной ролью.</summary>
        /// <param name="groupId">Идентификатор группы, в которую пользователь будет добавлен.</param>
        /// <param name="userName">Никнейм добавляемого пользователя.</param>
        /// <param name="role">Роль пользователя.</param>
        /// <param name="requestingUserId">Идентификатор пользователя, который делает данный запрос.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        Task<Result> AddMemberAsync(Guid groupId, string userName, Guid requestingUserId, string role = GroupRoles.Member);

        /// <summary>Получает список всех участников группы.</summary>
        /// <param name="groupId">Идентификатор группы, в которой нужно искать.</param>
        /// <param name="requestingUserId">Идентификатор пользователя, который запрашивает данные.</param>
        /// <returns>При успехе, возвращает успешный результат со списком всех участников группы.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с пустым списком и описанием ошибки.</returns>
        Task<Result<List<GroupMemberDto>>> GetGroupMembersAsync(Guid groupId, Guid requestingUserId);

        /// <summary>Получает количество участников в группе.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Количество участников.</returns>
        Task<Result<int>> GetMembersCountAsync(Guid groupId);

        /// <summary>Выдаёт права администратора члену группы.</summary>
        /// <param name="userId">Идентификатор пользователя, которому выдются права администратора.</param>
        /// <param name="groupId">Идентификатор группы, в которой дают участнику права администратора.</param>
        /// <param name="requestingUserId">Идентификатор пользователя, который делает данный запрос.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        Task<Result> PromoteToAdminAsync(Guid groupId, Guid userId, Guid requestingUserId);

        /// <summary>Изымает права администратора у администратора.</summary>
        /// <param name="groupId">Идентификатор группы, в которой изымают права администратора у участника.</param>
        /// <param name="userId">Идентификатор пользователя, у которого изымаются права администратора.</param>
        /// <param name="requestingUserId">Идентификатор пользователя, который делает данный запрос.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        Task<Result> DemoteToMemberAsync(Guid groupId, Guid userId, Guid requestingUserId);
        
        /// <summary>Удаляет пользователя из группы.</summary>
        /// <param name="groupId">Идентификатор группы из которой удаляется участник.</param>
        /// <param name="userName">Никнейм участника группы.</param>
        /// <param name="requestingUserId">Идентификатор пользователя, который запрашивает удаление участника.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        Task<Result> RemoveMemberAsync(Guid groupId, string userName, Guid requestingUserId);
    }
}
