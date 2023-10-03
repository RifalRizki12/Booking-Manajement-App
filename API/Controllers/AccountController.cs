using API.Contracts;
using API.DTOs.Accounts;
using API.Models;
using API.Repositories;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        // Konstruktor controller yang menerima IAccountRepository sebagai parameter.
        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
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
