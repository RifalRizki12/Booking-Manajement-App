using API.Contracts;
using API.Data;
using API.Models;

namespace API.Repositories
{
    public class RoomRepository : GeneralRepository<Room>, IRoomRepository
    {
        public RoomRepository(BookingManagementDbContext context) : base(context)
        {
        }

        public IEnumerable<Booking> GetRoomsInUse(DateTime date)
        {
            return _context.Bookings
                .Where(b => b.StartDate.Date <= date && date <= b.EndDate.Date)
                .ToList();
        }
    }
}
