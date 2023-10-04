using API.DTOs.Educations;
using FluentValidation;

namespace API.Utilities.Validation.Educations;

public class CreateEducationValidator : AbstractValidator<CreateEducationDto>
{
    public CreateEducationValidator()
    {
        // Aturan validasi untuk properti 'Major' dalam objek EducationDto
        RuleFor(e => e.Major)
            .NotEmpty()         // Properti tidak boleh kosong
            .MaximumLength(100);  // Panjang maksimal 100 karakter

        // Aturan validasi untuk properti 'Degree' dalam objek EducationDto
        RuleFor(e => e.Degree)
            .NotEmpty()         // Properti tidak boleh kosong
            .MaximumLength(100);  // Panjang maksimal 100 karakter

        // Aturan validasi untuk properti 'Gpa' dalam objek EducationDto
        RuleFor(e => e.Gpa)
            .NotNull()               // Properti tidak boleh null
            .InclusiveBetween(1, 4);  // Harus dalam rentang antara 0 dan 4

        // Aturan validasi untuk properti 'UniversityGuid' dalam objek EducationDto
        RuleFor(e => e.UniversityGuid)
            .NotEmpty();  // Properti tidak boleh kosong

        // Kembali, aturan validasi untuk properti 'Guid' dalam objek EducationDto
        // Ini mungkin merupakan duplikasi dan tidak diperlukan
        RuleFor(e => e.Guid)
            .NotEmpty();  // Properti tidak boleh kosong
    }
}