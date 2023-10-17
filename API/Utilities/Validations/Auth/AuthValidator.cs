using API.DTOs.Auth;
using FluentValidation;

namespace API.Utilities.Validations.Auth
{
    public class AuthValidator : AbstractValidator<LoginDto>
    {

        public AuthValidator()
        {
            // Aturan validasi untuk properti 'Email' dalam objek EmployeeDto
            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("Tidak Boleh Kosong")  // Properti tidak boleh kosong, dengan pesan kustom jika tidak terpenuhi
                .EmailAddress().WithMessage("Format Email Salah");  // Properti harus merupakan alamat email yang valid, dengan pesan kustom jika tidak terpenuhi

            // Aturan validasi untuk properti 'Password' dalam objek CreateAccountDto
            RuleFor(e => e.Password)
                .NotEmpty()         // Properti tidak boleh kosong
                .MinimumLength(8);  // Panjang minimal 8 karakter
        }
    }
}
