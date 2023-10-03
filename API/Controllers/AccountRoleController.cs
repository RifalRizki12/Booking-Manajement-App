// Mengimpor namespace yang diperlukan.
using API.Contracts;
using API.DTOs.AccountRoles;
using API.DTOs.Accounts;
using API.Models;
using Microsoft.AspNetCore.Mvc;

// Mendefinisikan controller dengan atribut ApiController dan route "/api/[controller]".
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountRoleController : ControllerBase
    {
        // Mendeklarasikan readonly field _accountRepository sebagai implementasi IAccountRoleRepository.
        private readonly IAccountRoleRepository _accountRepository;

        // Konstruktor controller yang menerima IAccountRoleRepository sebagai parameter.
        public AccountRoleController(IAccountRoleRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // HTTP GET endpoint untuk mengambil semua data AccountRole.
        [HttpGet]
        public IActionResult GetAll()
        {
            // Memanggil metode GetAll dari _accountRepository.
            var result = _accountRepository.GetAll();

            // Memeriksa apakah hasil query tidak mengandung data.
            if (!result.Any())
            {
                return NotFound("Data Not Found");
            }

            // Mengonversi hasil query ke objek DTO (Data Transfer Object) menggunakan Select.
            var data = result.Select(x => (AccountRoleDto)x);

            // Mengembalikan data yang ditemukan dalam respons OK.
            return Ok(data);
        }

        // HTTP GET endpoint untuk mengambil data AccountRole berdasarkan GUID.
        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            // Memanggil metode GetByGuid dari _accountRepository dengan parameter GUID.
            var result = _accountRepository.GetByGuid(guid);

            // Memeriksa apakah hasil query tidak ditemukan (null).
            if (result is null)
            {
                return NotFound("Id Not Found");
            }

            // Mengembalikan data yang ditemukan dalam respons OK.
            return Ok(result);
        }

        // HTTP POST endpoint untuk membuat data AccountRole baru.
        [HttpPost]
        public IActionResult Create(CreateAccountRoleDto accountRoleDto)
        {
            // Memanggil metode Create dari _accountRepository dengan parameter DTO.
            var result = _accountRepository.Create(accountRoleDto);

            // Memeriksa apakah penciptaan data berhasil atau gagal.
            if (result is null)
            {
                return BadRequest("Failed to create data");
            }

            // Mengembalikan data yang berhasil dibuat dalam respons OK.
            return Ok((AccountRoleDto)result);
        }

        // HTTP PUT endpoint untuk memperbarui data AccountRole.
        [HttpPut]
        public IActionResult Update(AccountRoleDto accountRoleDto)
        {
            // Memeriksa apakah entitas AccountRole yang akan diperbarui ada dalam database.
            var existingEmployee = _accountRepository.GetByGuid(accountRoleDto.Guid);

            if (existingEmployee == null)
            {
                return NotFound("Employee not found");
            }

            // Menyalin nilai CreatedDate dari entitas yang ada ke entitas yang akan diperbarui.
            AccountRole toUpdate = accountRoleDto;
            toUpdate.CreatedDate = existingEmployee.CreatedDate;

            // Memanggil metode Update dari _accountRepository.
            var result = _accountRepository.Update(toUpdate);

            // Memeriksa apakah pembaruan data berhasil atau gagal.
            if (!result)
            {
                return BadRequest("Failed to update data");
            }

            // Mengembalikan pesan sukses dalam respons OK.
            return Ok("Data Updated");
        }

        // HTTP DELETE endpoint untuk menghapus data AccountRole berdasarkan GUID.
        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            // Memanggil metode GetByGuid dari _accountRepository untuk mendapatkan entitas yang akan dihapus.
            var existingAccount = _accountRepository.GetByGuid(guid);

            // Memeriksa apakah entitas yang akan dihapus ada dalam database.
            if (existingAccount is null)
            {
                return NotFound("Account not found");
            }

            // Memanggil metode Delete dari _accountRepository.
            var deleted = _accountRepository.Delete(existingAccount);

            // Memeriksa apakah penghapusan data berhasil atau gagal.
            if (!deleted)
            {
                return BadRequest("Failed to delete account");
            }

            // Mengembalikan kode status 204 (No Content) untuk sukses penghapusan tanpa respons.
            return NoContent();
        }
    }
}
