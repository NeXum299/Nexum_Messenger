using FluentValidation;
using Server.Application.DTO;

namespace Server.Application.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        /// <summary>
        /// 
        /// </summary>
        public LoginModelValidator()
        {
            RuleFor(x => x.phoneNumber)
                .NotEmpty().WithMessage("Номер телефона обязателен")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Номер телефона должен быть в международном формате");

            RuleFor(x => x.password)
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
