using API.Contracts;
using API.DTOs.Employees;
using API.Models;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // GET api/employee
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _employeeRepository.GetAll();

            // Periksa jika ada karyawan dalam hasil
            if (!result.Any())
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Data Karyawan Tidak Ditemukan"
                });
            }

            // Konversi hasil ke EmployeeDto dan kembalikan dengan respons 200 OK
            var data = result.Select(x => (EmployeeDto)x);
            return Ok(new ResponseOKHandler<IEnumerable<EmployeeDto>>(data));
        }

        // GET api/employee/{guid}
        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            var result = _employeeRepository.GetByGuid(guid);

            // Periksa jika karyawan dengan GUID tertentu ada
            if (result is null)
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Karyawan dengan GUID Tertentu Tidak Ditemukan"
                });
            }

            // Konversi hasil ke EmployeeDto dan kembalikan dengan respons 200 OK
            return Ok(new ResponseOKHandler<EmployeeDto>((EmployeeDto)result));
        }

        // POST api/employee
        [HttpPost]
        public IActionResult Create(CreateEmployeeDto employeeDto)
        {
            try
            {

                // Buat instance Employee baru dari DTO yang diberikan
                Employee toCreate = employeeDto;

                // Hasilkan NIK baru dan berikan ke karyawan
                toCreate.Nik = GenerateHandler.generateNik(_employeeRepository.GetLastNik());

                // Panggil repositori untuk membuat karyawan
                var result = _employeeRepository.Create(toCreate);

                // Kembalikan karyawan yang telah dibuat dengan respons 200 OK
                return Ok(new ResponseOKHandler<EmployeeDto>((EmployeeDto)result));
            }
            catch (Exception ex)
            {
                // Tangani pengecualian dan kembalikan respons 500 Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Gagal membuat data employee",
                    Error = ex.Message
                });
            }
        }

        // PUT api/employee
        [HttpPut]
        public IActionResult Update(EmployeeDto employeeDto)
        {
            try
            {
                // Dapatkan entitas karyawan yang akan diperbarui berdasarkan GUID
                var entity = _employeeRepository.GetByGuid(employeeDto.Guid);

                // Periksa jika entitas ada
                if (entity is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Karyawan dengan GUID Tertentu Tidak Ditemukan"
                    });
                }

                // Ubah beberapa properti karyawan dan panggil repositori untuk memperbarui
                Employee toUpdate = employeeDto;
                toUpdate.Nik = entity.Nik;
                toUpdate.CreatedDate = entity.CreatedDate;

                _employeeRepository.Update(toUpdate);

                // Kembalikan pesan sukses dengan respons 200 OK
                return Ok(new ResponseOKHandler<string>("Data Karyawan Telah Diperbarui"));
            }
            catch (Exception ex)
            {
                // Tangani pengecualian dan kembalikan respons 500 Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Gagal memperbarui data karyawan",
                    Error = ex.Message
                });
            }
        }

        // DELETE api/employee/{guid}
        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            try
            {
                // Dapatkan entitas karyawan yang akan dihapus berdasarkan GUID
                var entity = _employeeRepository.GetByGuid(guid);

                // Periksa jika entitas ada
                if (entity is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Karyawan dengan GUID Tertentu Tidak Ditemukan"
                    });
                }

                // Hapus karyawan dari repositori
                _employeeRepository.Delete(entity);

                // Kembalikan pesan sukses dengan respons 200 OK
                return Ok(new ResponseOKHandler<string>("Data Karyawan Telah Dihapus"));
            }
            catch (ExceptionHandler ex)
            {
                // Tangani pengecualian dan kembalikan respons 500 Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Gagal menghapus data karyawan",
                    Error = ex.Message
                });
            }
        }
    }
}
