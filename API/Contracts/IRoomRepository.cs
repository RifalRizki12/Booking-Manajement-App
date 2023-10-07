using API.Models;

namespace API.Contracts
{
    public interface IRoomRepository : IGeneralRepository<Room>
    {
        IEnumerable<Booking> GetRoomsInUse(DateTime date);
    }
}
