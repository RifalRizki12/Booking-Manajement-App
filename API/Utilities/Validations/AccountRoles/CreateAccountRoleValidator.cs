using API.DTOs.AccountRoles;
using FluentValidation;

namespace API.Utilities.Validations.AccountRoles
{
    public class CreateAccountRoleValidator : AbstractValidator<CreateAccountRoleDto>
    {
        public CreateAccountRoleValidator() 
        {

            // Aturan validasi untuk properti 'AccountGuid' dalam objek AccountRoleDto
            RuleFor(e => e.AccountGuid)
                .NotEmpty();  // Properti tidak boleh kosong

            // Aturan validasi untuk properti 'RoleGuid' dalam objek AccountRoleDto
            RuleFor(e => e.RoleGuid)
                .NotEmpty();  // Properti tidak boleh kosong
        }
    }
}
