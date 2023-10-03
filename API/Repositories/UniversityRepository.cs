using API.Contracts;
using API.Data;
using API.Models;
using API.Repositories;

public class UniversityRepository : GeneralRepository<University>, IUniversityRepository
{
    public UniversityRepository(BookingManagementDbContext context) : base(context)
    {
        // Konstruktor UniversityRepository menerima instance BookingManagementDbContext
        // dan meneruskannya ke konstruktor base class GeneralRepository<University>.
        // Ini dilakukan untuk menginisialisasi repositori University dengan DbContext yang benar.
    }
}
