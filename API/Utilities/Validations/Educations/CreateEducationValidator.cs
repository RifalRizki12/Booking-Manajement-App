﻿using API.DTOs.Educations;
using FluentValidation;

namespace API.Utilities.Validation.Educations;

public class CreateEducationValidator : AbstractValidator<CreateEducationDto>
{
    public CreateEducationValidator()
    {
        RuleFor(e => e.Major)
           .NotEmpty()
           .MaximumLength(100);

        RuleFor(e => e.Degree)
           .NotEmpty()
           .MaximumLength(100);


        RuleFor(e => e.Gpa)
            .NotNull()
            .InclusiveBetween(0, 4);

        RuleFor(e => e.UniversityGuid)
           .NotEmpty();

        RuleFor(e => e.Guid)
        .NotEmpty();
    }
}