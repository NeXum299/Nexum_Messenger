using System;
using FluentValidation;
using Server.Core.Entities;

namespace Server.Application.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupMessageValidator : AbstractValidator<MessageGroupEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        public GroupMessageValidator()
        {
            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("Идентификатор группы не может быть пустым.");
                    
            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("Идентификатор отправителя не может быть пустым.");
                    
            RuleFor(x => x.Content)
                .NotEmpty().When(x => string.IsNullOrEmpty(x.AttachmentUrl))
                .WithMessage("Сообщение должно содержать текст или вложение.")
                .MaximumLength(2000).WithMessage("Сообщение не может быть длиннее 2000 символов.");
                    
            RuleFor(x => x.AttachmentUrl)
                .MaximumLength(500).WithMessage("Ссылка на вложение не может быть длиннее 500 символов.")
                .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.AttachmentUrl))
                .WithMessage("Некорректный формат ссылки на вложение.");
                    
            RuleFor(x => x.SentAt)
                .LessThanOrEqualTo(_ => DateTimeOffset.UtcNow)
                .WithMessage("Дата отправки не может быть в будущем.");
        }
    }
}
