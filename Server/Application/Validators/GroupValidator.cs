using System;
using FluentValidation;
using Server.Core.Entities;

namespace Server.Application.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupValidator : AbstractValidator<GroupEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        public GroupValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название группы обязательно для заполнения")
                .MaximumLength(50).WithMessage("Название группы не должно превышать 50 символов")
                .Matches("^[a-zA-Zа-яА-ЯёЁ0-9\\- _]+$").WithMessage("Название группы содержит недопустимые символы");

            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("Описание группы не должно превышать 200 символов");

            RuleFor(x => x.AvatarPath)
                .Must(path => path.StartsWith("/avatars/groups/"))
                .WithMessage("Аватар группы должен находиться в директории /avatars/groups/");

            /*RuleFor(x => x.CreatedAt)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Дата создания группы не может быть в будущем");*/

            RuleFor(x => x.CreatedBy)
                .NotNull().WithMessage("Создатель группы должен быть указан");

            RuleFor(x => x.CreatedById)
                .NotEmpty().WithMessage("Идентификатор создателя группы обязателен");
        }
    }
}
