using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Application.DTO;
using Server.Application.Exceptions;
using Server.Application.Interface.Repositories;
using Server.Application.Interface.Services;
using Server.Application.Validators;
using Server.Core.Entities;

namespace Server.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IUserRepository _userRepository;
        private readonly GroupValidator _groupValidator;
        private readonly UserValidator _userValidator;
        private readonly GroupMemberValidator _groupMemberValidator;
        private readonly ILogger<GroupService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupRepository"></param>
        /// <param name="groupValidator"></param>
        /// <param name="userValidator"></param>
        /// <param name="groupMemberValidator"></param>
        /// <param name="logger"></param>
        /// <param name="userRepository"></param>
        /// <param name="groupMemberRepository"></param>
        public GroupService(
            IGroupRepository groupRepository,
            GroupValidator groupValidator,
            UserValidator userValidator,
            GroupMemberValidator groupMemberValidator,
            ILogger<GroupService> logger,
            IUserRepository userRepository,
            IGroupMemberRepository groupMemberRepository)
        {
            _groupRepository = groupRepository;
            _groupValidator = groupValidator;
            _userValidator = userValidator;
            _groupMemberValidator = groupMemberValidator;
            _logger = logger;
            _userRepository = userRepository;
            _groupMemberRepository = groupMemberRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupDto"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        /// <exception cref="GroupException"></exception>
        public async Task<GroupDto> CreateGroupAsync(GroupDto groupDto, Guid creatorId)
        {
            try
            {
                var creator = await _userRepository.GetUserByIdAsync(creatorId);
                if (creator == null)
                    throw new GroupException("Ошибка создания группы");

                var validationUserResult = await _userValidator.ValidateAsync(creator);
                if (!validationUserResult.IsValid)
                    throw new ValidationException("Неверные данные пользователя при создании группы");

                var newGroup = new GroupEntity
                {
                    Id = groupDto.id,
                    Name = groupDto.name,
                    Description = groupDto.description,
                    AvatarPath = groupDto.avatarPath,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = creator,
                    CreatedById = creator.Id
                };

                var validationResult = await _groupValidator.ValidateAsync(newGroup);
                if (!validationResult.IsValid)
                    throw new ValidationException("Неверные данные группы при создании группы");

                var createdGroup = await _groupRepository.CreateGroupAsync(newGroup, creator);

                var createdGroupDto = new GroupDto
                (
                    createdGroup.Id,
                    createdGroup.Name!,
                    createdGroup.Description,
                    createdGroup.AvatarPath
                );

                return createdGroupDto;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error update database after create group.");
                throw new GroupException("Ошибка обновления базы данных после создания группы");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error update database after create group.");
                throw new GroupException("Ошибка обновления базы данных после создания группы");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        /// <exception cref="GroupException"></exception>
        public async Task<GroupDto> GetGroupByIdAsync(Guid groupId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new GroupException("Группа не найдена");

            var foundGroupDto = new GroupDto
            (
                group.Id,
                group.Name!,
                group.Description,
                group.AvatarPath
            );

            return foundGroupDto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="GroupException"></exception>
        public async Task<List<GroupDto>> GetAllGroupByGroupMemberId(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new GroupException("Пользователь не найден при запросе к списку групп");

            var groups = await _groupRepository.GetAllGroupByUserId(userId);

            List<GroupDto> groupsDto = new List<GroupDto>();

            foreach (var group in groups)
            {
                groupsDto.Add(new GroupDto
                (
                    group.Id,
                    group.Name!,
                    group.Description,
                    group.AvatarPath
                ));
            }

            return groupsDto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupDto"></param>
        /// <returns></returns>
        /// <exception cref="GroupException"></exception>
        public async Task<GroupDto> UpdateGroupAsync(GroupDto groupDto)
        {
            var updatedGroup = await _groupRepository.UpdateGroupAsync(groupDto);

            if (updatedGroup == null)
                throw new GroupException("Неверные данные перед обновлением");

            var updatedGroupDto = new GroupDto
            (
                updatedGroup.Id,
                updatedGroup.Name!,
                updatedGroup.Description,
                updatedGroup.AvatarPath
            );

            return updatedGroupDto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        /// <exception cref="GroupException"></exception>
        public async Task RemoveGroupAsync(Guid groupId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);

            if (group == null)
                throw new GroupException("Ошибка получения группы"); 

            await _groupRepository.DeleteGroupAsync(groupId);
        }
    }
}
