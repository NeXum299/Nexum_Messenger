using System;
using System.Linq;
using FluentValidation;
using ServerMessenger.Core.Entities;

namespace ServerMessenger.Application.Validators
{
    /// <summary>Валидатор для проверки объектов типа.<see cref="User"/>.</summary>
    public class UserValidator : AbstractValidator<User>
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UserValidator"/>
        /// с настройками правил валидации.
        /// </summary>
        public UserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Имя обязательно для заполнения")
                .Length(2, 25).WithMessage("Имя должно быть от 2 до 25 символов")
                .Matches("^[a-zA-Zа-яА-ЯёЁ\\- ]+$").WithMessage("Имя содержит недопустимые символы");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Фамилия обязательна для заполнения")
                .Length(2, 50).WithMessage("Фамилия должна быть от 2 до 50 символов")
                .Matches("^[a-zA-Zа-яА-ЯёЁ\\- ]+$").WithMessage("Фамилия содержит недопустимые символы");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Никнейм обязателен для заполнения")
                .Length(6, 70).WithMessage("Никнейм должна быть от 6 до 70 символов")
                .Matches("^[a-zA-Z0-9]+$").WithMessage("Никнйем содержит недопустимые символы");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Номер телефона обязателен")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Некорректный формат номера телефона");

            RuleFor(x => x.AvatarPath)
                .Must(path => path.StartsWith("/avatars/users/"))
                .WithMessage("Аватар должен находиться в директории /avatars/users/");

            RuleFor(x => x.BirthDate)
                .Must(bd => !bd.HasValue || (bd.Value.Year > 1900 && bd.Value < DateOnly.FromDateTime(DateTime.Today)))
                .WithMessage("Некорректная дата рождения");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Роль обязательна")
                .Must(role => new[] { "User", "Admin", "Moderator" }.Contains(role))
                .WithMessage("Недопустимая роль пользователя");
        }
    }
}
