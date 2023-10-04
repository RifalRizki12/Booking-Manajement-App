using API.DTOs.Rooms;
using FluentValidation;

namespace API.Utilities.Validations.Rooms;

public class RoomValidator : AbstractValidator<RoomDto>
{
    public RoomValidator()
    {
        RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(100);

        RuleFor(r => r.Floor)
            .NotNull()
            .WithMessage("Data floor harus berupa angka");

        RuleFor(r => r.Capacity)
            .NotNull();

    }

}