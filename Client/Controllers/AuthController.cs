using Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Client.Controllers
{
    public class AuthController : Controller
    {

        public IActionResult Login()
        {
            return View();
        }
    }
}