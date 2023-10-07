using API.Contracts;
using API.DTOs.Univers;
using API.DTOs.Universites;
using API.Models;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        // GET api/university
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                // Mengambil semua universitas dari repositori.
                var result = _universityRepository.GetAll();

                // Jika tidak ada universitas yang ditemukan, kembalikan respons "Data Not Found".
                if (!result.Any())
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Data Not Found"
                    });
                }

                // Mengonversi hasil ke DTO dan mengembalikan respons OK dengan data universitas.
                var data = result.Select(x => (UniversityDto)x);
                return Ok(new ResponseOKHandler<IEnumerable<UniversityDto>>(data));
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

        // GET api/university/{guid}
        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            try
            {
                // Mengambil universitas berdasarkan GUID yang diberikan dari repositori.
                var result = _universityRepository.GetByGuid(guid);

                // Jika universitas tidak ditemukan (hasil null), kembalikan respons "Id Not Found".
                if (result is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Id Not Found"
                    });
                }

                // Mengonversi hasil ke dalam bentuk DTO (UniversityDto) sebelum mengembalikan respons OK.
                return Ok(new ResponseOKHandler<UniversityDto>((UniversityDto)result));
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

        // POST api/university
        [HttpPost]
        public IActionResult Create(CreateUniversityDto universityDto)
        {
            try
            {
                // Memanggil metode Create dari _universityRepository dengan objek University baru.
                var result = _universityRepository.Create(universityDto);

                // Jika gagal membuat universitas, kembalikan respons "Failed to create data".
                if (result is null)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Failed to create data"
                    });
                }

                // Mengonversi hasil ke DTO dan mengembalikan respons OK dengan data universitas yang baru dibuat.
                return Ok(new ResponseOKHandler<UniversityDto>((UniversityDto)result));
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

        // PUT api/university
        [HttpPut]
        public IActionResult Update(UniversityDto universityDto)
        {
            try
            {
                // Mengambil entitas universitas berdasarkan GUID yang diberikan.
                var entity = _universityRepository.GetByGuid(universityDto.Guid);

                // Jika universitas tidak ditemukan, kembalikan respons "Id Not Found".
                if (entity is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Data Not Found"
                    });
                }

                // Menyimpan tanggal pembuatan entitas universitas sebelum pembaruan.
                University toUpdate = universityDto;
                toUpdate.CreatedDate = entity.CreatedDate;

                // Memperbarui data universitas dalam repositori.
                var result = _universityRepository.Update(toUpdate);

                // Jika gagal memperbarui data, kembalikan respons "Failed to update data".
                if (!result)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Failed to update data"
                    });
                }

                // Mengembalikan respons OK dengan pesan "Data Updated".
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

        // DELETE api/university/{guid}
        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            try
            {
                // Mengambil universitas berdasarkan GUID yang diberikan.
                var existingUniversity = _universityRepository.GetByGuid(guid);

                // Jika universitas tidak ditemukan, kembalikan respons "University not found".
                if (existingUniversity is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "University not found"
                    });
                }

                // Menghapus universitas dari repositori.
                var deleted = _universityRepository.Delete(existingUniversity);

                // Jika gagal menghapus universitas, kembalikan respons "Failed to delete university".
                if (!deleted)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Failed to delete university"
                    });
                }

                return Ok(new ResponseOKHandler<string>("Data University Is Delete"));
            }
            catch (ExceptionHandler ex)
            {
                // Jika terjadi pengecualian saat menghapus data, akan mengembalikan respons kesalahan dengan pesan pengecualian.
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to delete university",
                    Error = ex.Message
                });
            }
        }
    }
}
