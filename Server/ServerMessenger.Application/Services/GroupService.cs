using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerMessenger.Application.DTO;
using ServerMessenger.Application.Interface.Repositories;
using ServerMessenger.Application.Interface.Services;
using ServerMessenger.Application.Results;
using ServerMessenger.Application.Validators;
using ServerMessenger.Core.Entities;

namespace ServerMessenger.Application.Services
{
    /// <summary>Сервис для работы с группой.</summary>
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IUserRepository _userRepository;
        private readonly GroupValidator _groupValidator;
        private readonly UserValidator _userValidator;
        private readonly GroupMemberValidator _groupMemberValidator;
        private readonly ILogger<GroupService> _logger;
        private readonly IMapper _mapper;

        /// <summary>Конструктор инициализирующий <see cref="IGroupRepository"/>, <see cref="GroupValidator"/>,
        /// <see cref="UserValidator"/>> и <see cref="GroupMemberValidator"/>.</summary>
        /// <param name="groupRepository">Репозиторий для работы с группой.</param>
        /// <param name="userRepository">Репозиторий для работы с пользователем.</param>
        /// <param name="groupMemberRepository">Репозиторий для работы с участниками группы.</param>
        /// <param name="groupValidator">Валидатор для группы.</param>
        /// <param name="userValidator">Валидатор для пользователя.</param>
        /// <param name="groupMemberValidator">Валидатор для участник группы.</param>
        /// <param name="mapper">Маппер для группы и  DTO группы.</param>
        /// <param name="logger">Логирование для сервиса.</param>
        public GroupService(IGroupRepository groupRepository, GroupValidator groupValidator,
            UserValidator userValidator, GroupMemberValidator groupMemberValidator,
            ILogger<GroupService> logger, IUserRepository userRepository,
            IGroupMemberRepository groupMemberRepository, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _groupValidator = groupValidator;
            _userValidator = userValidator;
            _groupMemberValidator = groupMemberValidator;
            _logger = logger;
            _userRepository = userRepository;
            _groupMemberRepository = groupMemberRepository;
            _mapper = mapper;
        }

        /// <summary>Создаёт новую группу с указанным названием и описанием.</summary>
        /// <param name="groupDto">DTO создаваемой группы.</param>
        /// <param name="creatorId">Идентификатолр создателя группы.</param>
        /// <returns>При успехе, возвращает успешный результат с созданной группой.</returns>
        /// <returns>При ошибке, возвращает проваленный результат и null с описанием ошибки.</returns>
        public async Task<Result<GroupDto>> CreateGroupAsync(GroupDto groupDto, Guid creatorId)
        {
            try
            {
                var creatorResult = await _userRepository.GetUserByIdAsync(creatorId);
                if (creatorResult.Fail || creatorResult.Value == null)
                    return Result.Error<GroupDto>(null!, "Error creating group. May be him is null?");

                var validationUserResult = await _userValidator.ValidateAsync(creatorResult.Value);
                if (!validationUserResult.IsValid)
                {
                    var errors = validationUserResult.Errors.Select(e => e.ErrorMessage);
                    return Result.Error<GroupDto>(null!, errors);
                }

                var newGroup = _mapper.Map<Group>(groupDto);
                newGroup.CreatedBy = creatorResult.Value;
                newGroup.CreatedById = creatorResult.Value.Id;

                var validationResult = await _groupValidator.ValidateAsync(newGroup);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                    return Result.Error<GroupDto>(null!, errors);
                }

                var resultCreateGroup = await _groupRepository.CreateGroupAsync(newGroup, creatorResult.Value);

                var createdGroupDto = _mapper.Map<GroupDto>(resultCreateGroup.Value);

                return resultCreateGroup.Fail ? Result.Error<GroupDto>(null!, "Group creation Error")
                    : Result.Ok(createdGroupDto);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error update database after create group.");
                return Result.Error<GroupDto>(null!, "Error update database after create group.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error update database after create group.");
                return Result.Error<GroupDto>(null!, "Unexpected error.");
            }
        }

        /// <summary>Получает найденую группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Возвращает найденую группу.</returns>
        public async Task<Result<GroupDto>> GetGroupByIdAsync(Guid groupId)
        {
            var resultGroup = await _groupRepository.GetGroupByIdAsync(groupId);
            if (resultGroup.Fail)
                return Result.Error<GroupDto>(null!, "Group not found");

            var findGroupDto = _mapper.Map<GroupDto>(resultGroup.Value);

            return Result.Ok(findGroupDto);
        }

        /// <summary>Получает список всех групп, в которых есть участник.</summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Возвращает все найденные группы.</returns>
        public async Task<Result<List<GroupDto>>> GetAllGroupByGroupMemberId(Guid userId)
        {
            var userResult = await _userRepository.GetUserByIdAsync(userId);
            if (userResult.Fail || userResult.Value == null)
                return Result.Error(new List<GroupDto>(), "Error get the user");

            var groupsResult = await _groupRepository.GetAllGroupByUserId(userId);
            if (groupsResult.Fail)
                return Result.Error(new List<GroupDto>(), "Error get list groups by user id");

            var groupsDto = _mapper.Map<List<GroupDto>>(groupsResult.Value);

            return Result.Ok(groupsDto);
        }

        /// <summary>Обновляет группу.</summary>
        /// <param name="groupDto">DTO группы.</param>
        /// <returns>Обновлённый DTO группы.</returns>
        public async Task<Result<GroupDto>> UpdateGroupAsync(GroupDto groupDto)
        {
            var updateResult = await _groupRepository.UpdateGroupAsync(groupDto);
        
            if (updateResult.Fail || updateResult.Value == null)
                return Result.Error<GroupDto>(null!,
                    updateResult.Errors.FirstOrDefault() ?? "Error updating group");

            var updatedGroupDto = _mapper.Map<GroupDto>(updateResult.Value);

            return Result.Ok(updatedGroupDto);
        }

        /// <summary>Удаляет группу.</summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <returns>Результат операции.</returns>
        public async Task<Result> RemoveGroupAsync(Guid groupId)
        {
            var groupResult = await _groupRepository.GetGroupByIdAsync(groupId);
            if (groupResult.Fail || groupResult.Value == null)
                return Result.Error<GroupDto>(null!, "Error get the group"); 

            var removeResult = await _groupRepository.DeleteGroupAsync(groupId);
            if (removeResult.Fail)
                return Result.Error<GroupDto>(null!, "Error remove the group");

            return Result.Ok();
        }
    }
}
