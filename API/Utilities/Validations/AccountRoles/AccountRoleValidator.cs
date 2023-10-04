using API.DTOs.AccountRoles;
using API.DTOs.Roles;
using FluentValidation;

namespace API.Utilities.Validations.AccountRoles
{
    public class AccountRoleValidator : AbstractValidator<AccountRoleDto>
    {
        public AccountRoleValidator() 
        {
            RuleFor(e => e.Guid)
                .NotEmpty();

            RuleFor(e => e.AccountGuid)
                .NotEmpty();

            RuleFor(e => e.RoleGuid)
                .NotEmpty();
        }
    }
}
