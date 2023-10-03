using API.Contracts;
using API.DTOs.AccountRoles;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using API.Utilities.Handler;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountRoleController : ControllerBase
    {
        private readonly IAccountRoleRepository _accountRepository;

        public AccountRoleController(IAccountRoleRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // GET api/accountrole
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
                var data = result.Select(x => (AccountRoleDto)x);

                // Mengembalikan data yang ditemukan dalam respons OK.
                return Ok(new ResponseOKHandler<IEnumerable<AccountRoleDto>>(data));
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

        // GET api/accountrole/{guid}
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
                return Ok(new ResponseOKHandler<AccountRoleDto>((AccountRoleDto)result));
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

        // POST api/accountrole
        [HttpPost]
        public IActionResult Create(CreateAccountRoleDto accountRoleDto)
        {
            try
            {
                // Memanggil metode Create dari _accountRepository dengan parameter DTO.
                var result = _accountRepository.Create(accountRoleDto);

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
                return Ok(new ResponseOKHandler<AccountRoleDto>((AccountRoleDto)result));
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

        // PUT api/accountrole
        [HttpPut]
        public IActionResult Update(AccountRoleDto accountRoleDto)
        {
            try
            {
                // Memeriksa apakah entitas AccountRole yang akan diperbarui ada dalam database.
                var existingEmployee = _accountRepository.GetByGuid(accountRoleDto.Guid);

                if (existingEmployee == null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Account not found"
                    });
                }

                // Menyimpan tanggal pembuatan entitas AccountRole sebelum pembaruan.
                AccountRole toUpdate = accountRoleDto;
                toUpdate.CreatedDate = existingEmployee.CreatedDate;

                // Memanggil metode Update dari _accountRepository.
                var result = _accountRepository.Update(toUpdate);

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

        // DELETE api/accountrole/{guid}
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
