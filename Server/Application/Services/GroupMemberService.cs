using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Application.DTO;
using Server.Application.Interface.Repositories;
using Server.Application.Interface.Services;
using Server.Application.Results;
using Server.Application.Roles;
using Server.Application.Validators;

namespace Server.Application.Services
{
    /// <summary>Сервис для работы с участниками одной группы.</summary>
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly GroupValidator _groupValidator;
        private readonly UserValidator _userValidator;
        private readonly GroupMemberValidator _groupMemberValidator;
        private readonly IGroupMemberRepository _groupMemberRepository;

        /// <summary>Конструктор, который инициализирует локальные поля класса.</summary>
        /// <param name="groupRepository">Репозиторий группы.</param>
        /// <param name="userRepository">Репозиторий пользователей.</param>
        /// <param name="groupValidator">Валидатор группы.</param>
        /// <param name="userValidator">Валидатор пользователя.</param>
        /// <param name="groupMemberValidator">Валидатор участника.</param>
        /// <param name="groupMemberRepository">Репозиторий участников.</param>
        public GroupMemberService(IGroupRepository groupRepository, IUserRepository userRepository,
            UserValidator userValidator, GroupValidator groupValidator, GroupMemberValidator groupMemberValidator,
            IGroupMemberRepository groupMemberRepository)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _groupValidator = groupValidator;
            _userValidator = userValidator;
            _groupMemberRepository = groupMemberRepository;
            _groupMemberValidator = groupMemberValidator;
        }

        /// <summary>Добавляет пользователя в группу с указанной ролью.</summary>
        /// <param name="groupId">Идентификатор группы, в которую пользователь будет добавлен.</param>
        /// <param name="userName">Никнейм пользователя.</param>
        /// <param name="role">Роль пользователя.</param>
        /// <param name="requestingUserId">Идентификатор пользователя, который делает данный запрос.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        public async Task<Result> AddMemberAsync(Guid groupId, string userName, Guid requestingUserId,
            string role = GroupRoles.Member)
        {
            var isAdminOrOwner = await CheckOnAdminOrOwnerAsync(requestingUserId, groupId);
            if (isAdminOrOwner.Fail) return isAdminOrOwner;

            var groupResult = await _groupRepository.GetGroupByIdAsync(groupId);
            if (groupResult.Fail || groupResult.Value == null)
                return Result.Error("Error get the group");
            
            var userResult = await _userRepository.GetUserByUserNameAsync(userName);
            if (userResult.Fail || userResult.Value == null)
                return Result.Error("Error get the user");
            
            return await _groupMemberRepository
                .AddMemberAsync(groupResult.Value, userResult.Value, role);
        }

        /// <summary>Получает список всех участников группы.</summary>
        /// <param name="groupId">Идентификатор группы, в которой нужно искать.</param>
        /// <param name="requestingUserId">Идентификатор пользователя, который запрашивает данные.</param>
        /// <returns>При успехе, возвращает успешный результат со списком всех участников группы.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с пустым списком и описанием ошибки.</returns>
        public async Task<Result<List<GroupMemberDto>>> GetGroupMembersAsync(Guid groupId, Guid requestingUserId)
        {
            var isAdminOrOwner = await CheckOnAdminOrOwnerAsync(requestingUserId, groupId);
            if (isAdminOrOwner.Fail)
                return Result.Error(new List<GroupMemberDto>(), isAdminOrOwner.Errors);

            var groupResult = await _groupRepository.GetGroupByIdAsync(groupId);
            if (groupResult.Fail || groupResult.Value == null)
                return Result.Error(new List<GroupMemberDto>(), "Error get the group");

            var requestingUserResult = await _userRepository.GetUserByIdAsync(requestingUserId);
            if (requestingUserResult.Fail || requestingUserResult.Value == null)
                return Result.Error(new List<GroupMemberDto>(), "Error get the requesting user");

            var validationGroupResult = await _groupValidator.ValidateAsync(groupResult.Value);
            if (!validationGroupResult.IsValid)
            {
                var errors = validationGroupResult.Errors.Select(e => e.ErrorMessage);
                return Result.Error(new List<GroupMemberDto>(), errors);
            }

            var validationUserResult = await _userValidator.ValidateAsync(requestingUserResult.Value);
            if (!validationUserResult.IsValid)
            {
                var errors = validationUserResult.Errors.Select(e => e.ErrorMessage);
                return Result.Error(new List<GroupMemberDto>(), errors);
            }

            var result = await _groupMemberRepository.GetGroupMembersAsync(groupResult.Value);
            return result.Fail ? Result.Error(new List<GroupMemberDto>(), "Cannot found members in group") : Result.Ok(result.Value);
        }

        /// <summary>Получает количество участников в группе.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Количество участников.</returns>
        public async Task<Result<int>> GetMembersCountAsync(Guid groupId)
        {
            return await _groupMemberRepository.GetMembersCountAsync(groupId);
        }

        /// <summary>Выдаёт права администратора члену группы.</summary>
        /// <param name="userId">Идентификатор пользователя, которому выдются права администратора.</param>
        /// <param name="groupId">Идентификатор группы, в которой дают участнику права администратора.</param>
        /// <param name="requestingUserId">Идентификатор пользователя, который делает данный запрос.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        public async Task<Result> PromoteToAdminAsync(Guid groupId, Guid userId, Guid requestingUserId)
        {
            var isAdminOrOwner = await CheckOnAdminOrOwnerAsync(requestingUserId, groupId);
            if (isAdminOrOwner.Fail) return isAdminOrOwner;

            var groupResult = await _groupRepository.GetGroupByIdAsync(groupId);
            if (groupResult.Fail || groupResult.Value == null)
                return Result.Error("Error get the group"); 

            var userResult = await _userRepository.GetUserByIdAsync(userId);
            if (userResult.Fail || userResult.Value == null)
                return Result.Error("Error get the user");

            var validationGroupResult = await _groupValidator.ValidateAsync(groupResult.Value);
            if (!validationGroupResult.IsValid)
            {
                var errors = validationGroupResult.Errors.Select(e => e.ErrorMessage);
                return Result.Error(errors);
            }

            var validationUserResult = await _userValidator.ValidateAsync(userResult.Value);
            if (!validationUserResult.IsValid)
            {
                var errors = validationUserResult.Errors.Select(e => e.ErrorMessage);
                return Result.Error(errors);
            }

            var result = await _groupMemberRepository.PromoteToAdminAsync(groupResult.Value, userResult.Value);
            return result.Fail ? Result.Error("Is not possible member set to administrator role") : Result.Ok();
        }

        /// <summary>Изымает права администратора у администратора.</summary>
        /// <param name="groupId">Идентификатор группы, в которой изымают права администратора у участника.</param>
        /// <param name="userId">Идентификатор пользователя, у которого изымаются права администратора.</param>
        /// <param name="requestingUserId">Идентификатор пользователя, который делает данный запрос.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        public async Task<Result> DemoteToMemberAsync(Guid groupId, Guid userId, Guid requestingUserId)
        {
            var isAdminOrOwner = await CheckOnAdminOrOwnerAsync(requestingUserId, groupId);
            if (isAdminOrOwner.Fail) return isAdminOrOwner;

            var groupResult = await _groupRepository.GetGroupByIdAsync(groupId);
            if (groupResult.Fail || groupResult.Value == null)
                return Result.Error("Error get the group"); 

            var userResult = await _userRepository.GetUserByIdAsync(userId);
            if (userResult.Fail || userResult.Value == null)
                return Result.Error("Error get the user");

            var validationGroupResult = await _groupValidator.ValidateAsync(groupResult.Value);
            if (!validationGroupResult.IsValid)
            {
                var errors = validationGroupResult.Errors.Select(e => e.ErrorMessage);
                return Result.Error(errors);
            }

            var validationUserResult = await _userValidator.ValidateAsync(userResult.Value);
            if (!validationUserResult.IsValid)
            {
                var errors = validationUserResult.Errors.Select(e => e.ErrorMessage);
                return Result.Error(errors);
            }

            var result = await _groupMemberRepository.DemoteToMemberAsync(groupResult.Value, userResult.Value);
            return result.Fail ? Result.Error("Is not possible administrator set to member role") : Result.Ok();
        }
        
        /// <summary>Удаляет пользователя из группы.</summary>
        /// <param name="groupId">Идентификатор группы из которой удаляется участник.</param>
        /// <param name="userName">Никнейм участника группы.</param>
        /// <param name="requestingUserId">Идентификатор пользователя, который запрашивает удаление участника.</param>
        /// <returns>При успехе, возвращает успешный результат.</returns>
        /// <returns>При ошибке, возвращает проваленный результат с описанием ошибки.</returns>
        public async Task<Result> RemoveMemberAsync(Guid groupId, string userName, Guid requestingUserId)
        {
            var isAdminOrOwner = await CheckOnAdminOrOwnerAsync(requestingUserId, groupId);
            if (isAdminOrOwner.Fail) return isAdminOrOwner;

            var groupResult = await _groupRepository.GetGroupByIdAsync(groupId);
            if (groupResult.Fail || groupResult.Value == null)
                return Result.Error("Error get the group");

            var userResult = await _userRepository.GetUserByUserNameAsync(userName);
            if (userResult.Fail || userResult.Value == null)
                return Result.Error("Error get the user");

            var memberResult = await _groupMemberRepository
                .GetMemberByUserIdAndGroupIdAsync(userResult.Value.Id, groupId);
            if (memberResult.Fail || memberResult.Value == null)
                return Result.Error("User is not a member of this group");

            if (memberResult.Value.RoleInGroup == GroupRoles.Owner)
                return Result.Error("Cannot remove the owner of the group");

            return await _groupMemberRepository.RemoveMemberAsync(memberResult.Value);
        }

        private async Task<Result> CheckOnAdminOrOwnerAsync(Guid userId, Guid groupId)
        {
            var member = await _groupMemberRepository.GetMemberByUserIdAndGroupIdAsync(userId, groupId);
            
            if (member.Fail || member.Value == null)
                return Result.Error("Member not found");
            
            if (member.Value.RoleInGroup == GroupRoles.Admin || member.Value.RoleInGroup == GroupRoles.Owner)
                return Result.Ok();
            
            return Result.Error("Insufficient permissions");
        }
    }
}
