using API.Contracts;
using API.Data;
using API.Models;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

public class UniversityRepository : GeneralRepository<University>, IUniversityRepository
{
    private readonly BookingManagementDbContext _dbContext;
    public UniversityRepository(BookingManagementDbContext context) : base(context)
    {
        // Konstruktor UniversityRepository menerima instance BookingManagementDbContext
        // dan meneruskannya ke konstruktor base class GeneralRepository<University>.
        // Ini dilakukan untuk menginisialisasi repositori University dengan DbContext yang benar.

        _dbContext = context;
    }

    // Metode untuk mendapatkan objek University berdasarkan kode dan nama
    public University GetByCodeAndName(string code, string name)
    {
        // Menggunakan LINQ untuk mencari objek University pertama yang sesuai dengan kode dan nama yang diberikan
        // Objek _dbContext adalah instance dari DbContext yang digunakan untuk mengakses data
        return _dbContext.Universities.FirstOrDefault(u => u.Code == code && u.Name == name);
    }

}
