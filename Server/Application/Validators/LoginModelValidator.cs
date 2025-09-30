using FluentValidation;
using Server.Application.DTO;

namespace Server.Application.Validators
{
    /// <summary>Валидатор для проверки объектов типа <see cref="LoginModel"/>.</summary>
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="LoginModelValidator"/>
        /// с настройками правил валидации.
        /// </summary>
        public LoginModelValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Номер телефона обязателен")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Номер телефона должен быть в международном формате");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов")
                .MaximumLength(128).WithMessage("Пароль не должен превышать 128 символов")
                .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
                .Matches("[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву")
                .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру")
                .Matches("[^a-zA-Z0-9]").WithMessage("Пароль должен содержать хотя бы один специальный символ");
        }
    }
}
