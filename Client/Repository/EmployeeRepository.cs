using API.DTOs.Employees;
using API.Models;
using API.Utilities.Handler;
using Client.Contract;
using Newtonsoft.Json;
using System.Net.Http;

namespace Client.Repository
{
    public class EmployeeRepository : GeneralRepository<Employee, Guid>, IEmployeeRepository
    {

        public EmployeeRepository(string request = "Employee/") : base(request)
        {

        }

    }
}
