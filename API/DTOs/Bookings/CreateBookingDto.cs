using API.Models;

namespace API.DTOs.Bookings
{
    public class CreateBookingDto
    {
        // Properti-properti DTO untuk membuat objek Booking
        public DateTime StartDate { get; set; } // Tanggal dan waktu mulai booking
        public DateTime EndDate { get; set; } 
        public int Status { get; set; } 
        public string Remarks { get; set; } 
        public Guid RoomGuid { get; set; } // GUID yang merujuk ke ruangan yang dipesan
        public Guid EmployeeGuid { get; set; } // GUID yang merujuk ke karyawan yang melakukan pemesanan

        // Operator implisit digunakan untuk mengonversi objek CreateBookingDto ke objek Booking
        public static implicit operator Booking(CreateBookingDto bookingDto)
        {
            return new Booking
            {
                StartDate = bookingDto.StartDate, // Mengisi properti StartDate dengan nilai tanggal dan waktu mulai dari objek CreateBookingDto yang diberikan.
                EndDate = bookingDto.EndDate, 
                Status = bookingDto.Status, 
                Remarks = bookingDto.Remarks, 
                RoomGuid = bookingDto.RoomGuid, 
                EmployeeGuid = bookingDto.EmployeeGuid, 
                CreatedDate = DateTime.Now, // Mengisi properti CreatedDate dengan tanggal dan waktu saat ini.
                ModifiedDate = DateTime.Now // Mengisi properti ModifiedDate dengan tanggal dan waktu saat ini.
            };
        }
    }
}