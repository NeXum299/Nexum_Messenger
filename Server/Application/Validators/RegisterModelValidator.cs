using System;
using FluentValidation;
using Server.Application.DTO;

namespace Server.Application.Validators
{
    /// <summary>Валидатор для проверки объектов типа <see cref="RegisterModel"/>.</summary>
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="RegisterModelValidator"/>
        /// с настройками правил валидации.
        /// </summary>
        public RegisterModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Имя обязательно")
                .Length(2, 20).WithMessage("Имя должно содержать от 2 до 20 символов")
                .Matches(@"^[a-zA-Zа-яА-Я]+$").WithMessage("Имя может содержать только буквы");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Фамилия обязательна")
                .Length(2, 25).WithMessage("Фамилия должна содержать от 2 до 25 символов")
                .Matches(@"^[a-zA-Zа-яА-Я]+$").WithMessage("Фамилия может содержать только буквы");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Никнейм обязателен для заполнения")
                .Length(6, 70).WithMessage("Никнейм должна быть от 6 до 70 символов")
                .Matches("^[a-zA-Z0-9]+$").WithMessage("Никнйем содержит недопустимые символы");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Номер телефона обязателен")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Номер телефона должен быть в международном формате");

            RuleFor(x => x.AvatarPath)
                .NotEmpty().WithMessage("Путь к аватару обязателен")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.AvatarPath))
                .WithMessage("Неверный формат пути к аватару");

            RuleFor(x => x.BirthDate)
                .Must(BeValidDate).When(x => x.BirthDate.HasValue)
                .WithMessage("Некорректная дата рождения");

            RuleFor(x => x.Password)
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
