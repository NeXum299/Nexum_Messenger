using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Application.DTO;
using Server.Application.Results;
using Server.Application.Roles;
using Server.Core.Entities;

namespace Server.Application.Interface.Repositories
{
    /// <summary>Интерфейс для реализации репозитория, который будет управлять участниками в  какой то группе.</summary>
    public interface IGroupMemberRepository
    {
        /// <summary>Добавляет пользователя в группу с указанной ролью.</summary>
        /// <param name="group">Группа.</param>
        /// <param name="user">Добавляемый пользователь.</param>
        /// <param name="role">Роль добавляемого пользователя.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        Task<Result> AddMemberAsync(Group group, User user, string role = GroupRoles.Member);

        /// <summary>Получает определённого участника по идентфикатору.</summary>
        /// <param name="memberId">Идентификатор получаемого участника.</param>
        /// <returns>Возвращает участника в какой то группе.</returns>
        Task<Result<GroupMember>> GetGroupMemberByIdAsync(Guid memberId);

        /// <summary>Получает всех участников указанной группы.</summary>
        /// <param name="group">Группа.</param>
        /// <returns>При успехе, возвращает успешный результат со списком пользователей.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с пустым списком и описанием ошибки.</returns>
        Task<Result<List<GroupMemberDto>>> GetGroupMembersAsync(Group group);

        /// <summary>
        /// Получает определённого участника по идентификатору пользователя и идентификатору группы.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns>Возвращает участника группы.</returns>
        Task<Result<GroupMember>> GetMemberByUserIdAndGroupIdAsync(Guid userId, Guid groupId);

        /// <summary>Получает количество участников в группе.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Результат с количеством участников.</returns>
        Task<Result<int>> GetMembersCountAsync(Guid groupId);

        /// <summary>Даёт права администратора члену группы.</summary>
        /// <param name="group">Группа.</param>
        /// <param name="user">П0ользователь.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        Task<Result> PromoteToAdminAsync(Group group, User user);

        /// <summary>Изымает права администратора у администратора.</summary>
        /// <param name="group">Групп.</param>
        /// <param name="user">Пользователь.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        Task<Result> DemoteToMemberAsync(Group group, User user);

        /// <summary>Удаляет пользователя из группы.</summary>
        /// <param name="member">Удаляемый пользователь.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        Task<Result> RemoveMemberAsync(GroupMember member);
    }
}
