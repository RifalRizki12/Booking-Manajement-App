// Mengimpor namespace yang diperlukan.
using API.Contracts;
using API.DTOs.Roles;
using API.Models;
using Microsoft.AspNetCore.Mvc;

// Mendefinisikan controller dengan atribut ApiController dan route "/api/[controller]".
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        // Mendeklarasikan readonly field _roleRepository sebagai implementasi IRoleRepository.
        private readonly IRoleRepository _roleRepository;

        // Konstruktor controller yang menerima IRoleRepository sebagai parameter.
        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        // HTTP GET endpoint untuk mengambil semua data Role.
        [HttpGet]
        public IActionResult GetAll()
        {
            // Memanggil metode GetAll dari _roleRepository.
            var result = _roleRepository.GetAll();

            // Memeriksa apakah hasil query tidak mengandung data.
            if (!result.Any())
            {
                return NotFound("Data Not Found");
            }

            // Mengonversi hasil query ke objek DTO (Data Transfer Object) menggunakan Select.
            var data = result.Select(x => (RoleDto)x);

            // Mengembalikan data yang ditemukan dalam respons OK.
            return Ok(data);
        }

        // HTTP GET endpoint untuk mengambil data Role berdasarkan GUID.
        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            // Memanggil metode GetByGuid dari _roleRepository dengan parameter GUID.
            var result = _roleRepository.GetByGuid(guid);

            // Memeriksa apakah hasil query tidak ditemukan (null).
            if (result is null)
            {
                return NotFound("Id Not Found");
            }

            // Mengonversi hasil query ke objek DTO (Data Transfer Object).
            return Ok((RoleDto)result);
        }

        // HTTP POST endpoint untuk membuat data Role baru.
        [HttpPost]
        public IActionResult Create(CreateRoleDto roleDto)
        {
            // Memanggil metode Create dari _roleRepository dengan parameter DTO.
            var result = _roleRepository.Create(roleDto);

            // Memeriksa apakah penciptaan data berhasil atau gagal.
            if (result is null)
            {
                return BadRequest("Failed to create data");
            }

            // Mengembalikan data yang berhasil dibuat dalam respons OK.
            return Ok((RoleDto)result);
        }

        // HTTP PUT endpoint untuk memperbarui data Role.
        [HttpPut]
        public IActionResult Update(RoleDto roleDto)
        {
            // Memeriksa apakah entitas Role yang akan diperbarui ada dalam database.
            var entity = _roleRepository.GetByGuid(roleDto.Guid);
            if (entity is null)
            {
                return NotFound("Id Not Found");
            }

            // Menyalin nilai CreatedDate dari entitas yang ada ke entitas yang akan diperbarui.
            Role toUpdate = roleDto;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Memanggil metode Update dari _roleRepository.
            var result = _roleRepository.Update(toUpdate);

            // Memeriksa apakah pembaruan data berhasil atau gagal.
            if (!result)
            {
                return BadRequest("Failed to update data");
            }

            // Mengembalikan pesan sukses dalam respons OK.
            return Ok("Data Updated");
        }

        // HTTP DELETE endpoint untuk menghapus data Role berdasarkan GUID.
        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            // Memanggil metode GetByGuid dari _roleRepository untuk mendapatkan entitas yang akan dihapus.
            var existingRole = _roleRepository.GetByGuid(guid);

            // Memeriksa apakah entitas yang akan dihapus ada dalam database.
            if (existingRole is null)
            {
                return NotFound("Role not found");
            }

            // Memanggil metode Delete dari _roleRepository.
            var deleted = _roleRepository.Delete(existingRole);

            // Memeriksa apakah penghapusan data berhasil atau gagal.
            if (!deleted)
            {
                return BadRequest("Failed to delete role");
            }

            // Mengembalikan kode status 204 (No Content) untuk sukses penghapusan tanpa respons.
            return NoContent();
        }
    }
}
