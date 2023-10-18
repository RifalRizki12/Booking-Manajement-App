using API.DTOs.Employees;
using API.Models;
using Client.Contract;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Client.Controllers
{
    public class RoleController : Controller
    {

        private readonly IRoleRepository repository;

        public RoleController(IRoleRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        
        public async Task<JsonResult> GetAllRole()
        {
            var result = await repository.Get();
            return Json(result);
        }
        
    }
}