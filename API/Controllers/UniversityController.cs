using API.Contracts;
using API.DTOs.Univers;
using API.DTOs.Universites;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversityController : ControllerBase
    {
        private readonly IUniversityRepository _universityRepository;

        public UniversityController(IUniversityRepository universityRepository)
        {
            _universityRepository = universityRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Mengambil semua universitas dari repositori.
            var result = _universityRepository.GetAll();

            // Jika tidak ada universitas yang ditemukan, kembalikan respons "Data Not Found".
            if (!result.Any())
            {
                return NotFound("Data Not Found");
            }

            // Mengonversi hasil ke DTO dan mengembalikan respons OK dengan data universitas.
            var data = result.Select(x => (UniversityDto)x);
            return Ok(data);
        }

        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            // Mengambil universitas berdasarkan GUID yang diberikan dari repositori.
            var result = _universityRepository.GetByGuid(guid);

            // Jika universitas tidak ditemukan (hasil null), kembalikan respons "Id Not Found".
            if (result is null)
            {
                return NotFound("Id Not Found");
            }

            // Mengonversi hasil ke dalam bentuk DTO (UniversityDto) sebelum mengembalikan respons OK.
            return Ok((UniversityDto)result);
        }


        [HttpPost]
        public IActionResult Create(CreateUniversityDto universityDto)
        {
            // Memanggil metode Create dari _universityRepository dengan objek University baru.
            var result = _universityRepository.Create(universityDto);

            // Jika gagal membuat universitas, kembalikan respons "Failed to create data".
            if (result is null)
            {
                return BadRequest("Failed to create data");
            }

            // Mengonversi hasil ke DTO dan mengembalikan respons OK dengan data universitas yang baru dibuat.
            return Ok((UniversityDto)result);
        }

        [HttpPut]
        public IActionResult Update(UniversityDto universityDto)
        {
            // Mengambil entitas universitas berdasarkan GUID yang diberikan.
            var entity = _universityRepository.GetByGuid(universityDto.Guid);

            // Jika universitas tidak ditemukan, kembalikan respons "Id Not Found".
            if (entity is null)
            {
                return NotFound("Id Not Found");
            }

            // Menyimpan tanggal pembuatan entitas universitas sebelum pembaruan.
            University toUpdate = universityDto;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Memperbarui data universitas dalam repositori.
            var result = _universityRepository.Update(toUpdate);

            // Jika gagal memperbarui data, kembalikan respons "Failed to update data".
            if (!result)
            {
                return BadRequest("Failed to update data");
            }

            // Mengembalikan respons OK dengan pesan "Data Updated".
            return Ok("Data Updated");
        }

        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            // Mengambil universitas berdasarkan GUID yang diberikan.
            var existingUniversity = _universityRepository.GetByGuid(guid);

            // Jika universitas tidak ditemukan, kembalikan respons "University not found".
            if (existingUniversity is null)
            {
                return NotFound("University not found");
            }

            // Menghapus universitas dari repositori.
            var deleted = _universityRepository.Delete(existingUniversity);

            // Jika gagal menghapus universitas, kembalikan respons "Failed to delete university".
            if (!deleted)
            {
                return BadRequest("Failed to delete university");
            }

            // Mengembalikan kode status 204 No Content untuk sukses penghapusan tanpa respons.
            return NoContent();
        }
    }
}
