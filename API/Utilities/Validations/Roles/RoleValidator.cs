using API.DTOs.Roles;
using FluentValidation;

namespace API.Utilities.Validations.Roles
{
    public class RoleValidator : AbstractValidator<RoleDto>
    {
        public RoleValidator() 
        {
            RuleFor(e => e.Guid)
                .NotEmpty();

            RuleFor(e => e.Name)
               .NotEmpty()
               .MaximumLength(100);
        }
    }
}
