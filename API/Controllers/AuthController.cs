using API.Contracts;
using API.Data;
using API.DTOs.Accounts;
using API.DTOs.Auth;
using API.Models;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Transactions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[AllowAnonymous]
    public class AuthController : ControllerBase
    {
        
        private readonly IAccountRepository _accountRepository; // Gantilah dengan layanan yang mengatur token
        private readonly ITokenHandler _tokenHandler;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;
        private readonly IRoleRepository _roleRepository;

        public AuthController(IAccountRepository tokenService, ITokenHandler tokenHandler, IEmployeeRepository employeeRepository, IAccountRoleRepository accountRoleRepository, IRoleRepository roleRepository)
        {

            _accountRepository = tokenService;
            _tokenHandler = tokenHandler;
            _employeeRepository = employeeRepository;
            _accountRoleRepository = accountRoleRepository;
            _roleRepository = roleRepository;
        }

        // Metode untuk autentikasi pengguna saat login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto request)
        {
            try
            {
                // Validasi input data menggunakan ModelState
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Invalid input data."
                    });
                }

                // Cari pengguna (akun) berdasarkan alamat email
                var user = _accountRepository.GetByEmployeeEmail(request.Email);
                var employee = _employeeRepository.GetByEmployeeEmail(request.Email);

                if (user == null || !HashHandler.VerifyPassword(request.Password, user.Password))
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Account or Password is invalid.",
                    });
                }

                var claims = new List<Claim>();
                claims.Add(new Claim("Email", employee.Email));
                claims.Add(new Claim("Fullname", string.Concat(employee.FirstName + " " + employee.LastName)));

                var getRoleName = from ar in _accountRoleRepository.GetAll()
                                  join r in _roleRepository.GetAll() on ar.RoleGuid equals r.Guid
                                  where ar.AccountGuid == user.Guid
                                  select r.Name;

                foreach (var roleName in getRoleName)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleName));
                }

                var generateToken = _tokenHandler.Generate(claims);

                // Jika validasi berhasil, kirim respons OK dengan pesan login berhasil
                return Ok(new ResponseOKHandler<object>("Login Success", new { Token = generateToken }));
            }
            catch (Exception ex)
            {
                // Tangani pengecualian dan kembalikan respons 500 Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Error during login",
                    Error = ex.Message
                });
            }
        }

    }
}
