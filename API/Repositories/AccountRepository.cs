using API.Contracts;
using API.Data;
using API.Models;

namespace API.Repositories
{
    public class AccountRepository : GeneralRepository<Account>, IAccountRepository
    {
        private readonly BookingManagementDbContext _context;

        // Konstruktor AccountRepository yang menerima BookingManagementDbContext
        public AccountRepository(BookingManagementDbContext context) : base(context)
        {
            _context = context;
        }

        // Metode untuk mendapatkan akun berdasarkan alamat email karyawan
        public Account GetByEmployeeEmail(string employeeEmail)
        {
            // Melakukan join antara tabel Akun (Account) dan Employee berdasarkan email Employee
            var account = _context.Accounts
                .Join(
                    _context.Employees,
                    account => account.Guid,
                    employee => employee.Guid,
                    (account, employee) => new
                    {
                        Account = account,
                        Employee = employee
                    }
                )
                // Memfilter hasil join berdasarkan alamat email karyawan
                .Where(joinResult => joinResult.Employee.Email == employeeEmail)
                // Memilih objek akun sebagai hasil akhir
                .Select(joinResult => joinResult.Account)
                .FirstOrDefault();

            return account;
        }
    }
}
