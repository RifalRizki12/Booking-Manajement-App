using API.Contracts;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class EmployeeRepository : GeneralRepository<Employee>, IEmployeeRepository
    {
        private readonly BookingManagementDbContext _context;
        public EmployeeRepository(BookingManagementDbContext context) : base(context)
        {
            _context = context;
        }

        public string GetLastNik()
        {
            return _context.Employees
                           .OrderByDescending(e => e.Nik)
                           .Select(e => e.Nik)
                           .FirstOrDefault();
        }

        /*public Employee GetByEmployeeEmail(string employeeEmail)
        {
            // Implementasi metode GetByEmployeeEmail di sini
            // Menggunakan LINQ untuk mencari karyawan berdasarkan email
            return _context.Employees.FirstOrDefault(employee => employee.Email == employeeEmail);
        }*/
    }
}
