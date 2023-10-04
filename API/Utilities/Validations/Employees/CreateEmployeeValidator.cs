﻿using API.DTOs.Employees;
using FluentValidation;

namespace API.Utilities.Validations.Employees;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeValidator()
    {
        RuleFor(e => e.FirstName)
           .NotEmpty();

        RuleFor(employee => employee.BirthDate)
                .NotEmpty().WithMessage("Tanggal lahir wajib diisi.")
                .Must(birthDate => (DateTime.Today - birthDate).TotalDays / 365 >= 18)
                .WithMessage("Anda harus berusia minimal 18 tahun.");

        RuleFor(e => e.Gender)
           .NotNull()
           .IsInEnum();

        RuleFor(e => e.HiringDate).NotEmpty();

        RuleFor(e => e.Email)
           .NotEmpty().WithMessage("Tidak Boleh Kosong")
           .EmailAddress().WithMessage("Format Email Salah");

        RuleFor(e => e.PhoneNumber)
           .NotEmpty()
           .MaximumLength(20);
    }
}