using System;
using FluentValidation;
using ServerMessenger.Core.Entities;

namespace ServerMessenger.Application.Validators
{
    /// <summary>Валидатор для проверки объектов типа <see cref="GroupMember"/>.</summary>
    public class GroupMemberValidator : AbstractValidator<GroupMember>
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GroupMemberValidator"/>
        /// с настройками правил валидации.
        /// </summary>
        public GroupMemberValidator()
        {
            RuleFor(x => x.Group)
                .NotNull().WithMessage("Группа должна быть указана");

            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("Идентификатор группы обязателен");

            RuleFor(x => x.User)
                .NotNull().WithMessage("Пользователь должен быть указан");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Идентификатор пользователя обязателен");

            RuleFor(x => x.JoinedAt)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Дата присоединения не может быть в будущем");

            RuleFor(x => x.RoleInGroup)
                .NotEmpty().WithMessage("Роль в группе обязательна")
                .MaximumLength(20).WithMessage("Роль в группе не должна превышать 20 символов")
                .Matches("^[a-zA-Zа-яА-ЯёЁ0-9\\- ]+$").WithMessage("Роль в группе содержит недопустимые символы");
        }
    }
}
