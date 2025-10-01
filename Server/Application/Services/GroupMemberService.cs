using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Application.DTO;
using Server.Application.Exceptions;
using Server.Application.Interface.Repositories;
using Server.Application.Interface.Services;
using Server.Application.Roles;
using Server.Application.Validators;

namespace Server.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly GroupValidator _groupValidator;
        private readonly UserValidator _userValidator;
        private readonly GroupMemberValidator _groupMemberValidator;
        private readonly IGroupMemberRepository _groupMemberRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="userValidator"></param>
        /// <param name="groupValidator"></param>
        /// <param name="groupMemberValidator"></param>
        /// <param name="groupMemberRepository"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userName"></param>
        /// <param name="requestingUserId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task AddMemberAsync(
            Guid groupId,
            string userName,
            Guid requestingUserId,
            string role)
        {
            var isAdminOrOwner = await CheckOnAdminOrOwnerAsync(requestingUserId, groupId);
            if (!isAdminOrOwner) return;

            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new GroupException("Ошибка получения группы");
            
            var user = await _userRepository.GetUserByUserNameAsync(userName);
            if (user == null)
                throw new UserException("Error get the user");
            
            await _groupMemberRepository.AddMemberAsync(group, user, role);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="requestingUserId"></param>
        /// <returns></returns>
        public async Task<List<GroupMemberDto>> GetGroupMembersAsync(Guid groupId, Guid requestingUserId)
        {
            var isAdminOrOwner = await CheckOnAdminOrOwnerAsync(requestingUserId, groupId);
            if (!isAdminOrOwner) throw new GroupException(
                "Вы не являетесь администратором данной группы");

            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new GroupException("Группа не найдена");

            var validationGroupResult = await _groupValidator.ValidateAsync(group);
            if (!validationGroupResult.IsValid)
                throw new GroupException("Неверные данные группы");

            return await _groupMemberRepository.GetGroupMembersAsync(group);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<int> GetMembersCountAsync(Guid groupId)
        {
            return await _groupMemberRepository.GetMembersCountAsync(groupId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="requestingUserId"></param>
        /// <returns></returns>
        public async Task PromoteToAdminAsync(Guid groupId, Guid userId, Guid requestingUserId)
        {
            var isAdminOrOwner = await CheckOnAdminOrOwnerAsync(requestingUserId, groupId);
            if (!isAdminOrOwner) return;

            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new GroupException("Ошибка получения группы"); 

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new UserException("Ошибка получения пользователя");

            var validationGroupResult = await _groupValidator.ValidateAsync(group);
            if (!validationGroupResult.IsValid)
                throw new ValidationException("Неверные данные группы");

            var validationUserResult = await _userValidator.ValidateAsync(user);
            if (!validationUserResult.IsValid)
                throw new ValidationException("Неверные данные пользователя");

            await _groupMemberRepository.PromoteToAdminAsync(group, user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="requestingUserId"></param>
        /// <returns></returns>
        public async Task DemoteToMemberAsync(Guid groupId, Guid userId, Guid requestingUserId)
        {
            var isAdminOrOwner = await CheckOnAdminOrOwnerAsync(requestingUserId, groupId);
            if (!isAdminOrOwner) return;

            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new GroupException("Ошибка получения группы"); 

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new UserException("Ошибка получения пользователя");

            var validationGroupResult = await _groupValidator.ValidateAsync(group);
            if (!validationGroupResult.IsValid)
                throw new ValidationException("");

            var validationUserResult = await _userValidator.ValidateAsync(user);
            if (!validationUserResult.IsValid)
                throw new ValidationException("");

            await _groupMemberRepository.DemoteToMemberAsync(group, user);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userName"></param>
        /// <param name="requestingUserId"></param>
        /// <returns></returns>
        public async Task RemoveMemberAsync(Guid groupId, string userName, Guid requestingUserId)
        {
            var isAdminOrOwner = await CheckOnAdminOrOwnerAsync(requestingUserId, groupId);
            if (!isAdminOrOwner) return;

            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new GroupException("Ошибка получения группы");

            var user = await _userRepository.GetUserByUserNameAsync(userName);
            if (user == null)
                throw new UserException("Ошибка получения пользователя");

            var member = await _groupMemberRepository
                .GetMemberByUserIdAndGroupIdAsync(user.Id, groupId);
            if (member == null)
                throw new GroupMemberException("Ошибка получения участника группы");

            if (member.RoleInGroup == GroupRoles.Owner.ToString())
                throw new GroupMemberException("Не удается удалить владельца группы");

            await _groupMemberRepository.RemoveMemberAsync(member);
        }

        private async Task<bool> CheckOnAdminOrOwnerAsync(Guid userId, Guid groupId)
        {
            var member = await _groupMemberRepository
                .GetMemberByUserIdAndGroupIdAsync(userId, groupId);

            if (member == null)
                throw new GroupMemberException("Участник не найден");
            
            if (member.RoleInGroup == GroupRoles.Admin.ToString()
                || member.RoleInGroup == GroupRoles.Owner.ToString())
                return true;

            return false;
        }
    }
}
