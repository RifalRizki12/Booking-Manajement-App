using API.DTOs.Bookings;
using FluentValidation;

namespace API.Utilities.Validations.Bookings
{
    public class CreateBookingValidator : AbstractValidator<CreateBookingDto>
    {
        public CreateBookingValidator() 
        {
            RuleFor(b => b.StartDate)
                .NotEmpty();

            RuleFor(b => b.EndDate)
                .NotEmpty();

            RuleFor(b => b.Status)
                .NotNull()
                .IsInEnum();

            RuleFor(b => b.Remarks)
                .NotNull();

            RuleFor(b => b.RoomGuid)
                .NotEmpty();

            RuleFor(b => b.EmployeeGuid)
                .NotEmpty();

        }
    }
}
