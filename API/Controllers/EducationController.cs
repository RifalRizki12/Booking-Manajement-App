// Mengimpor namespace yang diperlukan.
using API.Contracts;
using API.DTOs.Educations;
using API.DTOs.Employees;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

// Mendefinisikan controller dengan atribut ApiController dan route "/api/[controller]".
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationController : ControllerBase
    {
        // Mendeklarasikan readonly field _educationRepository sebagai implementasi IEducationRepository.
        private readonly IEducationRepository _educationRepository;

        // Konstruktor controller yang menerima IEducationRepository sebagai parameter.
        public EducationController(IEducationRepository educationRepository)
        {
            _educationRepository = educationRepository;
        }

        // HTTP GET endpoint untuk mengambil semua data Education.
        [HttpGet]
        public IActionResult GetAll()
        {
            // Memanggil metode GetAll dari _educationRepository.
            var result = _educationRepository.GetAll();

            // Memeriksa apakah hasil query tidak mengandung data.
            if (!result.Any())
            {
                return NotFound("Data Not Found");
            }

            // Mengonversi hasil query ke objek DTO (Data Transfer Object) menggunakan Select.
            var data = result.Select(x => (EducationDto)x);

            // Mengembalikan data yang ditemukan dalam respons OK.
            return Ok(data);
        }

        // HTTP GET endpoint untuk mengambil data Education berdasarkan GUID.
        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            // Memanggil metode GetByGuid dari _educationRepository dengan parameter GUID.
            var result = _educationRepository.GetByGuid(guid);

            // Memeriksa apakah hasil query tidak ditemukan (null).
            if (result is null)
            {
                return NotFound("Id Not Found");
            }

            // Mengembalikan data yang ditemukan dalam respons OK.
            return Ok(result);
        }

        // HTTP POST endpoint untuk membuat data Education baru.
        [HttpPost]
        public IActionResult Create(CreateEducationDto educationDto)
        {
            // Memanggil metode Create dari _educationRepository dengan parameter DTO.
            var result = _educationRepository.Create(educationDto);

            // Memeriksa apakah penciptaan data berhasil atau gagal.
            if (result is null)
            {
                return BadRequest("Failed to create data");
            }

            // Mengembalikan data yang berhasil dibuat dalam respons OK.
            return Ok((EducationDto)result);
        }

        // HTTP PUT endpoint untuk memperbarui data Education.
        [HttpPut]
        public IActionResult Update(EducationDto educationDto)
        {
            // Memeriksa apakah entitas Education yang akan diperbarui ada dalam database.
            var existingEmployee = _educationRepository.GetByGuid(educationDto.Guid);

            if (existingEmployee == null)
            {
                return NotFound("Employee not found");
            }

            // Menyalin nilai CreatedDate dari entitas yang ada ke entitas yang akan diperbarui.
            Education toUpdate = educationDto;
            toUpdate.CreatedDate = existingEmployee.CreatedDate;

            // Memanggil metode Update dari _educationRepository.
            var result = _educationRepository.Update(toUpdate);

            // Memeriksa apakah pembaruan data berhasil atau gagal.
            if (!result)
            {
                return BadRequest("Failed to update data");
            }

            // Mengembalikan pesan sukses dalam respons OK.
            return Ok("Data Updated");
        }

        // HTTP DELETE endpoint untuk menghapus data Education berdasarkan GUID.
        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            // Memanggil metode GetByGuid dari _educationRepository untuk mendapatkan entitas yang akan dihapus.
            var existingEducation = _educationRepository.GetByGuid(guid);

            // Memeriksa apakah entitas yang akan dihapus ada dalam database.
            if (existingEducation is null)
            {
                return NotFound("Education not found");
            }

            // Memanggil metode Delete dari _educationRepository.
            var deleted = _educationRepository.Delete(existingEducation);

            // Memeriksa apakah penghapusan data berhasil atau gagal.
            if (!deleted)
            {
                return BadRequest("Failed to delete education");
            }

            // Mengembalikan kode status 204 (No Content) untuk sukses penghapusan tanpa respons.
            return NoContent();
        }
    }
}
