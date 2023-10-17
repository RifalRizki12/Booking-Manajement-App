using API.DTOs.Employees;
using API.Models;
using API.Utilities.Handler;

namespace Client.Contract
{
    public interface IEmployeeRepository : IRepository<Employee, Guid>
    {

        /*Task<ResponseOKHandler<EmployeeDto>> Delete(Guid guid);*/
    }
}
