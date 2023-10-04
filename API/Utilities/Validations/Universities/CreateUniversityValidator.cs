using API.DTOs.Universites;
using FluentValidation;

namespace API.Utilities.Validation.Universitys;

public class CreateUniversityValidator : AbstractValidator<CreateUniversityDto>
{
    public CreateUniversityValidator()
    {
        RuleFor(e => e.Code)
           .NotEmpty()
           .MaximumLength(50);

        RuleFor(e => e.Name)
           .NotEmpty()
           .MaximumLength(100);
    }
}