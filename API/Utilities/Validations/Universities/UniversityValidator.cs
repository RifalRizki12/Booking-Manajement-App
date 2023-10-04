using API.DTOs.Univers;
using FluentValidation;

namespace API.Utilities.Validation.Universitys;

public class UniversityValidator : AbstractValidator<UniversityDto>
{
    public UniversityValidator()
    {
        RuleFor(e => e.Guid)
           .NotEmpty();

        RuleFor(e => e.Code)
           .NotEmpty()
           .MaximumLength(50);

        RuleFor(e => e.Name)
           .NotEmpty()
           .MaximumLength(100);
    }
}