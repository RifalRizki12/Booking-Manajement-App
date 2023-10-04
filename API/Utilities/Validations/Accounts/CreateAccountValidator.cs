using API.DTOs.Accounts;
using FluentValidation;

namespace API.Utilities.Validations.Accounts
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountDto>
    {
        public CreateAccountValidator() 
        {
            RuleFor(e => e.Password)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(e => e.Otp)
                .NotEmpty();

            RuleFor(e => e.IsUsed)
                .NotEmpty();

            RuleFor(e => e.ExpiredTime)
                .NotEmpty()
                .Must(expiredTime => expiredTime > DateTime.Now);
        }
    }
}
