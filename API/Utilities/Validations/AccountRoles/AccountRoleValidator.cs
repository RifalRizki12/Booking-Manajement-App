using API.DTOs.AccountRoles;
using FluentValidation;

namespace API.Utilities.Validations.AccountRoles
{
    // Mendefinisikan kelas AccountRoleValidator yang menggantungkan tipe AccountRoleDto
    public class AccountRoleValidator : AbstractValidator<AccountRoleDto>
    {
        // Konstruktor kelas AccountRoleValidator
        public AccountRoleValidator()
        {
            // Aturan validasi untuk properti 'Guid' dalam objek AccountRoleDto
            RuleFor(e => e.Guid)
                .NotEmpty();  // Properti tidak boleh kosong

            // Aturan validasi untuk properti 'AccountGuid' dalam objek AccountRoleDto
            RuleFor(e => e.AccountGuid)
                .NotEmpty();  // Properti tidak boleh kosong

            // Aturan validasi untuk properti 'RoleGuid' dalam objek AccountRoleDto
            RuleFor(e => e.RoleGuid)
                .NotEmpty();  // Properti tidak boleh kosong
        }
    }
}
