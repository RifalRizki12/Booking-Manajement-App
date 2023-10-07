using API.Utilities.Enums;

namespace API.DTOs.Bookings
{
    public class BookingDetailDto
    {
        public Guid Guid { get; set; }
        public string BookedNIK { get; set; }
        public string BookedBy { get; set; }
        public string RoomName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public StatusLevel Status { get; set; }
        public string Remarks { get; set; }
    }
}
