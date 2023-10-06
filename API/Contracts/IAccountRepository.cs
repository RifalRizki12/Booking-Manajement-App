using API.Models;

namespace API.Contracts
{
    public interface IAccountRepository : IGeneralRepository<Account>
    {
        Account GetByEmployeeEmail(string employeeEmail);
    }

}
