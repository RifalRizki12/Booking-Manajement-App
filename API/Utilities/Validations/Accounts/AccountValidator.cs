using API.DTOs.Accounts;
using FluentValidation;

namespace API.Utilities.Validations.Accounts
{
    public class AccountValidator : AbstractValidator<AccountDto>
    {
        public AccountValidator() 
        {
            RuleFor(e => e.Guid)
                .NotEmpty();

            // Aturan validasi untuk properti 'Password' dalam objek CreateAccountDto
            RuleFor(e => e.Password)
                .NotEmpty()         // Properti tidak boleh kosong
                .MinimumLength(8);  // Panjang minimal 8 karakter

            // Aturan validasi untuk properti 'Otp' dalam objek CreateAccountDto
            RuleFor(e => e.Otp)
                .NotEmpty();        // Properti tidak boleh kosong

            // Aturan validasi untuk properti 'IsUsed' dalam objek CreateAccountDto
            RuleFor(e => e.IsUsed)
                .NotEmpty();        // Properti tidak boleh kosong

            // Aturan validasi untuk properti 'ExpiredTime' dalam objek CreateAccountDto
            RuleFor(e => e.ExpiredTime)
                .NotEmpty()                 // Properti tidak boleh kosong
                .Must(expiredTime => expiredTime > DateTime.Now);  // Harus lebih besar dari waktu saat ini
        }
    }
}
