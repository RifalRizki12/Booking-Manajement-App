using API.Contracts;
using API.Data;
using API.DTOs.Accounts;
using API.DTOs.Auth;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Transactions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        
        private readonly IAccountRepository _accountRepository; // Gantilah dengan layanan yang mengatur token

        public AuthController(IAccountRepository tokenService)
        {
            
            _accountRepository = tokenService;
        }

        // Metode untuk autentikasi pengguna saat login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto request)
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

            if (user == null || !HashHandler.VerifyPassword(request.Password, user.Password))
            {
                return BadRequest(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = "Account or Password is invalid."
                });
            }

            // Jika validasi berhasil, kirim respons OK dengan pesan login berhasil
            return Ok(new ResponseOKHandler<IEnumerable<string>>("Login Sucsess"));
        }
    }
}
