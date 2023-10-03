using API.Contracts;
using API.DTOs.Employees;
using API.Models;
using Microsoft.AspNetCore.Mvc;

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
            // Mengambil daftar semua karyawan dari repository.
            var result = _employeeRepository.GetAll();

            // Jika tidak ada data karyawan, kembalikan respons Not Found.
            if (!result.Any())
            {
                return NotFound("Data Not Found");
            }

            // Mengkonversi daftar karyawan menjadi daftar EmployeeDto dan mengembalikan respons OK.
            var data = result.Select(x => (EmployeeDto)x);
            return Ok(result);
        }

        // GET api/employee/{guid}
        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            // Mengambil karyawan berdasarkan GUID yang diberikan dari repository.
            var result = _employeeRepository.GetByGuid(guid);

            // Jika karyawan tidak ditemukan, kembalikan respons Not Found.
            if (result is null)
            {
                return NotFound("Id Not Found");
            }

            // Mengkonversi hasil ke EmployeeDto dan mengembalikan respons OK.
            return Ok((EmployeeDto)result);
        }

        // POST api/employee
        [HttpPost]
        public IActionResult Create(CreateEmployeeDto employeeDto)
        {
            // Membuat karyawan baru berdasarkan data yang diberikan.
            var result = _employeeRepository.Create(employeeDto);

            // Jika pembuatan karyawan gagal, kembalikan respons BadRequest.
            if (result is null)
            {
                return BadRequest("Failed to create data");
            }

            // Mengkonversi hasil ke EmployeeDto dan mengembalikan respons OK.
            return Ok((EmployeeDto)result);
        }

        // PUT api/employee
        [HttpPut]
        public IActionResult Update(EmployeeDto employeeDto)
        {
            // Mengambil karyawan yang sudah ada berdasarkan GUID yang diberikan.
            var existingEmployee = _employeeRepository.GetByGuid(employeeDto.Guid);

            // Jika karyawan tidak ditemukan, kembalikan respons Not Found.
            if (existingEmployee == null)
            {
                return NotFound("Employee not found");
            }

            // Mengupdate properti karyawan yang ada dengan data dari EmployeeDto.
            Employee toUpdate = employeeDto;
            toUpdate.CreatedDate = existingEmployee.CreatedDate;

            // Melakukan update data karyawan ke repository.
            var result = _employeeRepository.Update(toUpdate);

            // Jika update gagal, kembalikan respons BadRequest.
            if (!result)
            {
                return BadRequest("Failed to update data");
            }

            // Kembalikan respons OK dengan pesan "Data Updated".
            return Ok("Data Updated");
        }

        // DELETE api/employee/{guid}
        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            // Mengambil karyawan berdasarkan GUID yang diberikan.
            var existingEmployee = _employeeRepository.GetByGuid(guid);

            // Jika karyawan tidak ditemukan, kembalikan respons Not Found.
            if (existingEmployee is null)
            {
                return NotFound("Employee not found");
            }

            // Menghapus karyawan dari repository.
            var deleted = _employeeRepository.Delete(existingEmployee);

            // Jika penghapusan gagal, kembalikan respons BadRequest.
            if (!deleted)
            {
                return BadRequest("Failed to delete employee");
            }

            // Kembalikan respons NoContent (status 204) untuk sukses penghapusan tanpa respons.
            return NoContent();
        }
    }
}
