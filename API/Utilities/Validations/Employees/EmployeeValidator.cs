using API.Models;
using FluentValidation;
using API.Utilities.Validations;
using API.DTOs.Employees;

namespace API.Utilities.Validations.Employees
{
    public class EmployeeValidator : AbstractValidator<EmployeeDto>
    {
        public EmployeeValidator()
        {
            RuleFor(employee => employee.BirthDate)
                .NotEmpty().WithMessage("Tanggal lahir wajib diisi.")
                .Must(birthDate => (DateTime.Today - birthDate).TotalDays / 365 >= 18)
                .WithMessage("Anda harus berusia minimal 18 tahun.");

            RuleFor(employee => employee.Email)
                .NotEmpty().WithMessage("Email wajib diisi.")
                .EmailAddress().WithMessage("Format email tidak valid.");

            RuleFor(employee => employee.PhoneNumber)
                .NotEmpty().WithMessage("Nomor telepon wajib diisi.")
                .Matches(@"^\d{10,15}$").WithMessage("Format nomor telepon tidak valid.");
        }
    }
}
