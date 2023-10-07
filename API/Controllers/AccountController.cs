using API.Contracts;
using API.DTOs.Accounts;
using API.DTOs.Employees;
using API.Models;
using API.Repositories;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Transactions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailHandler _emailHandler;

        // Konstruktor controller yang menerima IAccountRepository sebagai parameter.
        public AccountController(IAccountRepository accountRepository, IEmployeeRepository employeeRepository, IEmailHandler emailHandler)
        {
            _accountRepository = accountRepository;
            _employeeRepository = employeeRepository;
            _emailHandler = emailHandler;
        }

        // Metode untuk mengirim OTP melalui email dalam kasus lupa kata sandi
        [HttpPut("forgot-password")]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string email)
        {
            var employees = _employeeRepository.GetAll();
            var accounts = _accountRepository.GetAll();

            // Memeriksa apakah ada data karyawan dan akun
            if (!(employees.Any() && accounts.Any()))
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Data Tidak Ditemukan"
                });
            }

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = "Invalid input data."
                });
            }

            // Cari akun berdasarkan email
            var account = _accountRepository.GetByEmployeeEmail(email);

            if (account == null)
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Email not found."
                });
            }

            // Generate OTP dengan tipe data int
            Random random = new Random();
            int otp = random.Next(100000, 999999);

            // Update akun dengan OTP yang baru dan expired time 30 menit dari sekarang
            account.Otp = otp;
            account.ModifiedDate = DateTime.UtcNow;
            account.ExpiredTime = DateTime.UtcNow.AddMinutes(30); // Set expired time 30 menit dari sekarang
            account.IsUsed = false; // Set ulang IsUsed menjadi false karena ini adalah OTP baru
            _accountRepository.Update(account);

            _emailHandler.Send("Forgot Password", $"Your OTP is {account.Otp}", email);

            // Melakukan join antara tabel Employee, Education, dan University
            var accountDetails = from ac in accounts
                                 join emp in employees on ac.Guid equals emp.Guid
                                 select new ForgotPasswordDto
                                 {
                                     Email = emp.Email,
                                     Otp = ac.Otp,
                                     IsUsed = ac.IsUsed,
                                     ExpiredTime = ac.ExpiredTime,
                                 };

            return Ok(new ResponseOKHandler<IEnumerable<ForgotPasswordDto>>(accountDetails));
        }

        // Metode untuk mengganti kata sandi
        [HttpPut("change-password")]
        [AllowAnonymous]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto request)
        {
            var employees = _employeeRepository.GetAll();
            var accounts = _accountRepository.GetAll();

            // Memeriksa apakah ada data karyawan dan akun
            if (!(employees.Any() && accounts.Any()))
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Data Tidak Ditemukan"
                });
            }

            // Cari akun berdasarkan email
            var account = _accountRepository.GetByEmployeeEmail(request.Email);

            if (account == null)
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Email not found."
                });
            }

            // Memeriksa OTP
            if (account.Otp != request.Otp)
            {
                return BadRequest(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = "Invalid OTP."
                });
            }

            // Memeriksa apakah OTP sudah digunakan
            if (account.IsUsed)
            {
                return Conflict(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status409Conflict,
                    Status = HttpStatusCode.Conflict.ToString(),
                    Message = "OTP has already been used."
                });
            }

            // Memeriksa apakah OTP sudah kadaluwarsa
            if (account.ExpiredTime != null && account.ExpiredTime <= DateTime.UtcNow)
            {
                return BadRequest(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = "OTP has expired."
                });
            }

            // Memeriksa apakah NewPassword dan ConfirmPassword cocok
            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = "NewPassword and ConfirmPassword do not match."
                });
            }

            // Hash password baru menggunakan bcrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // Update password di akun sebenarnya di database
            account.Password = hashedPassword;
            account.ModifiedDate = DateTime.UtcNow;
            account.IsUsed = true; // Set IsUsed menjadi true karena OTP sudah digunakan
            _accountRepository.Update(account);

            string responseMessage = "Password has been changed successfully.";

            return Ok(new ResponseOKHandler<string>(responseMessage));
        }

        // GET api/account
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                // Memanggil metode GetAll dari _accountRepository.
                var result = _accountRepository.GetAll();

                // Memeriksa apakah hasil query tidak mengandung data.
                if (!result.Any())
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Data Not Found"
                    });
                }

                // Mengonversi hasil query ke objek DTO (Data Transfer Object) menggunakan Select.
                var data = result.Select(x => (AccountDto)x);

                // Mengembalikan data yang ditemukan dalam respons OK.
                return Ok(new ResponseOKHandler<IEnumerable<AccountDto>>(data));
            }
            catch (ExceptionHandler ex)
            {
                // Jika terjadi pengecualian saat mengambil data, akan mengembalikan respons kesalahan dengan pesan pengecualian.
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to retrieve data",
                    Error = ex.Message
                });
            }
        }

        // GET api/account/{guid}
        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            try
            {
                // Memanggil metode GetByGuid dari _accountRepository dengan parameter GUID.
                var result = _accountRepository.GetByGuid(guid);

                // Memeriksa apakah hasil query tidak ditemukan (null).
                if (result is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Id Not Found"
                    });
                }

                // Mengembalikan data yang ditemukan dalam respons OK.
                return Ok(new ResponseOKHandler<AccountDto>((AccountDto)result));
            }
            catch (ExceptionHandler ex)
            {
                // Jika terjadi pengecualian saat mengambil data, akan mengembalikan respons kesalahan dengan pesan pengecualian.
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to retrieve data",
                    Error = ex.Message
                });
            }
        }

        // POST api/account
        [HttpPost]
        public IActionResult Create(CreateAccountDto accountDto)
        {
            try
            {
                // Meng-hash kata sandi sebelum menyimpannya ke database.
                string hashedPassword = HashHandler.HashPassword(accountDto.Password);

                // Mengganti kata sandi asli dengan yang di-hash sebelum menyimpannya ke DTO.
                accountDto.Password = hashedPassword;

                // Memanggil metode Create dari _accountRepository dengan parameter DTO yang sudah di-hash.
                var result = _accountRepository.Create(accountDto);

                // Memeriksa apakah penciptaan data berhasil atau gagal.
                if (result is null)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Failed to create data"
                    });
                }

                // Mengembalikan data yang berhasil dibuat dalam respons OK.
                return Ok(new ResponseOKHandler<AccountDto>((AccountDto)result));
            }
            catch (ExceptionHandler ex)
            {
                // Jika terjadi pengecualian saat membuat data, akan mengembalikan respons kesalahan dengan pesan pengecualian.
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to create data",
                    Error = ex.Message
                });
            }
        }

        // PUT api/account
        [HttpPut]
        public IActionResult Update(AccountDto accountDto)
        {
            try
            {
                // Memeriksa apakah entitas Account yang akan diperbarui ada dalam database.
                var entity = _accountRepository.GetByGuid(accountDto.Guid);
                if (entity is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Id Not Found"
                    });
                }

                // Memeriksa apakah kata sandi berubah.
                if (!string.IsNullOrEmpty(accountDto.Password))
                {
                    // Meng-hash kata sandi baru sebelum menyimpannya ke database.
                    string hashedPassword = HashHandler.HashPassword(accountDto.Password);

                    // Menyalin nilai CreatedDate dari entitas yang ada ke entitas yang akan diperbarui.
                    Account toUpdate = accountDto;
                    toUpdate.CreatedDate = entity.CreatedDate;

                    // Mengganti kata sandi asli dengan yang di-hash pada objek entity.
                    entity.Password = hashedPassword;
                }

                // Memanggil metode Update dari _accountRepository.
                var result = _accountRepository.Update(entity);

                // Memeriksa apakah pembaruan data berhasil atau gagal.
                if (!result)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Failed to update data"
                    });
                }

                // Mengembalikan pesan sukses dalam respons OK.
                return Ok(new ResponseOKHandler<string>("Data Updated"));
            }
            catch (ExceptionHandler ex)
            {
                // Jika terjadi pengecualian saat mengupdate data, akan mengembalikan respons kesalahan dengan pesan pengecualian.
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to update data",
                    Error = ex.Message
                });
            }
        }

        // DELETE api/account/{guid}
        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            try
            {
                // Memanggil metode GetByGuid dari _accountRepository untuk mendapatkan entitas yang akan dihapus.
                var existingAccount = _accountRepository.GetByGuid(guid);

                // Memeriksa apakah entitas yang akan dihapus ada dalam database.
                if (existingAccount is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Account not found"
                    });
                }

                // Memanggil metode Delete dari _accountRepository.
                var deleted = _accountRepository.Delete(existingAccount);

                // Memeriksa apakah penghapusan data berhasil atau gagal.
                if (!deleted)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Failed to delete account"
                    });
                }

                // Mengembalikan kode status 204 (No Content) untuk sukses penghapusan tanpa respons.
                return NoContent();
            }
            catch (ExceptionHandler ex)
            {
                // Jika terjadi pengecualian saat menghapus data, akan mengembalikan respons kesalahan dengan pesan pengecualian.
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to delete account",
                    Error = ex.Message
                });
            }
        }
    }
}
