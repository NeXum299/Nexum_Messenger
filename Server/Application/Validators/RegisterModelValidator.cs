using System;
using FluentValidation;
using Server.Application.DTO;

namespace Server.Application.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        /// <summary>
        /// 
        /// </summary>
        public RegisterModelValidator()
        {
            RuleFor(x => x.firstName)
                .NotEmpty().WithMessage("Имя обязательно")
                .Length(2, 20).WithMessage("Имя должно содержать от 2 до 20 символов")
                .Matches(@"^[a-zA-Zа-яА-Я]+$").WithMessage("Имя может содержать только буквы");

            RuleFor(x => x.lastName)
                .NotEmpty().WithMessage("Фамилия обязательна")
                .Length(2, 25).WithMessage("Фамилия должна содержать от 2 до 25 символов")
                .Matches(@"^[a-zA-Zа-яА-Я]+$").WithMessage("Фамилия может содержать только буквы");

            RuleFor(x => x.userName)
                .NotEmpty().WithMessage("Никнейм обязателен для заполнения")
                .Length(6, 70).WithMessage("Никнейм должна быть от 6 до 70 символов")
                .Matches("^[a-zA-Z0-9]+$").WithMessage("Никнйем содержит недопустимые символы");

            RuleFor(x => x.phoneNumber)
                .NotEmpty().WithMessage("Номер телефона обязателен")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Номер телефона должен быть в международном формате");

            RuleFor(x => x.avatarPath)
                .NotEmpty().WithMessage("Путь к аватару обязателен")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.avatarPath))
                .WithMessage("Неверный формат пути к аватару");

            RuleFor(x => x.birthDate)
                .Must(BeValidDate).When(x => x.birthDate.HasValue)
                .WithMessage("Некорректная дата рождения");

            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов")
                .MaximumLength(128).WithMessage("Пароль не должен превышать 128 символов")
                .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
                .Matches("[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву")
                .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру")
                .Matches("[^a-zA-Z0-9]").WithMessage("Пароль должен содержать хотя бы один специальный символ");
        }

        private bool BeValidDate(DateOnly? date)
        {
            if (!date.HasValue) return true;
            return date.Value.Year > 1900 && date.Value < DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
