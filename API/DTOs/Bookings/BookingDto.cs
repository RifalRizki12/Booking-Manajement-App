using API.Models;

namespace API.DTOs.Bookings
{
    public class BookingDto
    {
        // Properti-properti DTO untuk objek Booking
        public Guid Guid { get; set; } // GUID unik untuk booking
        public DateTime StartDate { get; set; } // Tanggal dan waktu mulai booking
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; }
        public Guid RoomGuid { get; set; } // GUID yang merujuk ke ruangan yang dipesan
        public Guid EmployeeGuid { get; set; } // GUID yang merujuk ke karyawan yang melakukan pemesanan

        // Operator eksplisit untuk mengonversi objek Booking ke BookingDto
        public static explicit operator BookingDto(Booking booking)
        {
            return new BookingDto
            {
                Guid = booking.Guid, // Mengisi properti Guid dengan nilai GUID dari objek Booking yang diberikan.
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                Status = booking.Status,
                Remarks = booking.Remarks,
                RoomGuid = booking.RoomGuid, // Mengisi properti RoomGuid dengan nilai GUID ruangan dari objek Booking yang diberikan.
                EmployeeGuid = booking.EmployeeGuid // Mengisi properti EmployeeGuid dengan nilai GUID karyawan dari objek Booking yang diberikan.
            };
        }

        // Operator implisit untuk mengonversi objek BookingDto ke Booking (untuk operasi update)
        public static implicit operator Booking(BookingDto bookingDto)
        {
            return new Booking
            {
                Guid = bookingDto.Guid, // Mengisi properti Guid dengan nilai GUID dari objek BookingDto yang diberikan.
                StartDate = bookingDto.StartDate,
                EndDate = bookingDto.EndDate,
                Status = bookingDto.Status,
                Remarks = bookingDto.Remarks,
                RoomGuid = bookingDto.RoomGuid, // Mengisi properti RoomGuid dengan nilai GUID ruangan dari objek BookingDto yang diberikan.
                EmployeeGuid = bookingDto.EmployeeGuid, // Mengisi properti EmployeeGuid dengan nilai GUID karyawan dari objek BookingDto yang diberikan.
                ModifiedDate = DateTime.Now // Mengisi properti ModifiedDate dengan tanggal dan waktu saat ini.
            };
        }
    }
}