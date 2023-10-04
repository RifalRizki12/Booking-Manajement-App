using API.DTOs.Rooms;
using FluentValidation;

namespace API.Utilities.Validations.Rooms;

public class UpdateRoomValidator : AbstractValidator<RoomDto>
{
    public UpdateRoomValidator()
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