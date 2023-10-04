using API.DTOs.Bookings;
using FluentValidation;

namespace API.Utilities.Validations.Bookings
{
    public class BookingValidator : AbstractValidator<BookingDto>
    {

        public BookingValidator()
        {
            RuleFor(b => b.Guid)
                .NotEmpty();

            // Aturan validasi untuk properti 'StartDate' dalam objek CreateBookingDto
            RuleFor(b => b.StartDate)
                .NotEmpty();  // Properti tidak boleh kosong

            // Aturan validasi untuk properti 'EndDate' dalam objek CreateBookingDto
            RuleFor(b => b.EndDate)
                .NotEmpty();  // Properti tidak boleh kosong

            // Aturan validasi untuk properti 'Status' dalam objek CreateBookingDto
            RuleFor(b => b.Status)
                .NotNull()     // Properti tidak boleh null
                .IsInEnum();   // Properti harus merupakan nilai dari enum yang valid

            // Aturan validasi untuk properti 'Remarks' dalam objek CreateBookingDto
            RuleFor(b => b.Remarks)
                .NotNull();    // Properti tidak boleh null

            // Aturan validasi untuk properti 'RoomGuid' dalam objek CreateBookingDto
            RuleFor(b => b.RoomGuid)
                .NotEmpty();   // Properti tidak boleh kosong

            // Aturan validasi untuk properti 'EmployeeGuid' dalam objek CreateBookingDto
            RuleFor(b => b.EmployeeGuid)
                .NotEmpty();   // Properti tidak boleh kosong

        }
    }
}