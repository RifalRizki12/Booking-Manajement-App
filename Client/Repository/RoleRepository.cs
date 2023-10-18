using API.DTOs.Employees;
using API.Models;
using API.Utilities.Handler;
using Client.Contract;
using Newtonsoft.Json;
using System.Net.Http;

namespace Client.Repository
{
    public class RoleRepository : GeneralRepository<Role, Guid>, IRoleRepository
    {

        public RoleRepository(string request = "Role/") : base(request)
        {

        }

    }
}
