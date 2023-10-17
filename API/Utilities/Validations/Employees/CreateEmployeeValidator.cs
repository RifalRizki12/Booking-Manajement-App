using API.DTOs.Employees;
using FluentValidation;

namespace API.Utilities.Validations.Employees;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
{
    // Konstruktor kelas CreateEmployeeValidator
    public CreateEmployeeValidator()
    {
        // Aturan validasi untuk properti 'FirstName' dalam objek EmployeeDto
        RuleFor(e => e.FirstName)
            .NotEmpty().WithMessage("first name tidak boleh kosong");  // Properti tidak boleh kosong

        // Aturan validasi untuk properti 'BirthDate' dalam objek EmployeeDto
        RuleFor(employee => employee.BirthDate)
            .NotNull().WithMessage("Tanggal lahir wajib diisi.")  // Properti tidak boleh kosong, dengan pesan kustom jika tidak terpenuhi
            .Must(birthDate => (DateTime.Today - birthDate).TotalDays / 365 >= 18)
            .WithMessage("Anda harus berusia minimal 18 tahun.");  // Properti harus memenuhi kondisi usia minimal 18 tahun, dengan pesan kustom jika tidak terpenuhi

        // Aturan validasi untuk properti 'Gender' dalam objek EmployeeDto
        RuleFor(e => e.Gender)
            .NotNull()     // Properti tidak boleh null
            .IsInEnum();   // Properti harus merupakan nilai dari enum yang valid

        // Aturan validasi untuk properti 'HiringDate' dalam objek EmployeeDto
        RuleFor(employee => employee.HiringDate)
            .NotNull().WithMessage("Hiring Date tidak boleh kosong.")
            .Must(date => date != DateTime.MinValue).WithMessage("Hiring Date harus diisi.");


        // Aturan validasi untuk properti 'Email' dalam objek EmployeeDto
        RuleFor(e => e.Email)
            .NotEmpty().WithMessage("Tidak Boleh Kosong")  // Properti tidak boleh kosong, dengan pesan kustom jika tidak terpenuhi
            .EmailAddress().WithMessage("Format Email Salah");  // Properti harus merupakan alamat email yang valid, dengan pesan kustom jika tidak terpenuhi

        // Aturan validasi untuk properti 'PhoneNumber' dalam objek EmployeeDto
        RuleFor(e => e.PhoneNumber)
            .NotEmpty().WithMessage("Phone Number tidak boleh kosong")         // Properti tidak boleh kosong
            .MaximumLength(20); // Panjang maksimal 20 karakter
    }
}