using API.Contracts;
using API.Data;
using API.DTOs.Accounts;
using API.DTOs.Educations;
using API.DTOs.Employees;
using API.DTOs.Univers;
using API.DTOs.Universites;
using API.Models;
using API.Repositories;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Transactions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEducationRepository _educationRepository;
        private readonly IUniversityRepository _universityRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IBookingRepository _bookingRepository;

        public EmployeeController(IEmployeeRepository employeeRepository, IEducationRepository educationRepository, IUniversityRepository universityRepository, IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository, IRoleRepository roleRepository, IBookingRepository bookingRepository)
        {
            _employeeRepository = employeeRepository;
            _educationRepository = educationRepository;
            _universityRepository = universityRepository;
            _accountRepository = accountRepository;
            _accountRoleRepository = accountRoleRepository;
            _roleRepository = roleRepository;
            _bookingRepository = bookingRepository;
        }

        // Metode untuk mendaftarkan pengguna baru
        [HttpPost("register")]
        //[AllowAnonymous]
        public IActionResult Register([FromBody] RegisterDto request)
        {
            // Validasi apakah kata sandi dan konfirmasi kata sandi cocok
            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = "Password and ConfirmPassword do not match."
                });
            }

            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    // Cek apakah universitas sudah ada di database berdasarkan code dan name
                    var existingUniversity = _universityRepository.GetByCodeAndName(request.UniversityCode, request.UniversityName);

                    if (existingUniversity == null)
                    {
                        // Universitas belum ada, maka buat baru
                        University newUniversityEntity = request; // Menggunakan operator konversi implisit

                        // Memanggil metode Create dari _universityRepository dengan objek University baru
                        var resultUni = _universityRepository.Create(newUniversityEntity);

                        // Menghubungkan universitas yang baru dibuat dengan existingUniversity
                        existingUniversity = newUniversityEntity;
                    }

                    // Hash password menggunakan bcrypt
                    string hashedPassword = HashHandler.HashPassword(request.Password);

                    // Konversi RegisterDto ke Employee entity menggunakan operator konversi implisit
                    Employee newEmployeeEntity = request;
                    // Generate NIK
                    newEmployeeEntity.Nik = GenerateHandler.generateNik(_employeeRepository.GetLastNik());

                    // Simpan Employee dalam repository
                    var resultEmp = _employeeRepository.Create(newEmployeeEntity);

                    // Hubungkan Employee dengan Education
                    Education newEducationEntity = request; // Menggunakan operator konversi implisit
                    newEducationEntity.Guid = resultEmp.Guid;
                    newEducationEntity.UniversityGuid = existingUniversity.Guid;
                    var resultEdu = _educationRepository.Create(newEducationEntity);

                    // Buat objek Account dari RegisterDto
                    Account newAccountEntity = request; // Menggunakan operator konversi implisit
                    newAccountEntity.Password = hashedPassword;
                    newAccountEntity.Guid = newEmployeeEntity.Guid;

                    // Simpan Account dalam repository
                    var resultAcc = _accountRepository.Create(newAccountEntity);

                    //Generate add role user
                    var accountRole = _accountRoleRepository.Create(new AccountRole
                    {
                        AccountGuid = newEmployeeEntity.Guid,
                        RoleGuid = _roleRepository.GetDefaultGuid() ?? throw new Exception("Default role not found")
                    });

                    // Commit transaksi jika semua operasi berhasil
                    transactionScope.Complete();

                    return Ok(new ResponseOKHandler<string>("Registration successful."));
                }
                catch (Exception ex)
                {
                    // Rollback transaksi jika terjadi kesalahan
                    transactionScope.Dispose();

                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Registration failed. " + ex.Message
                    });
                }
            }
        }

        // Metode untuk mendapatkan detail karyawan
        [HttpGet("details")]
        //[AllowAnonymous]
        public IActionResult GetDetails()
        {
            var employees = _employeeRepository.GetAll();
            var educations = _educationRepository.GetAll();
            var universities = _universityRepository.GetAll();

            if (!(employees.Any() && educations.Any() && universities.Any()))
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Data Employee Tidak Ditemukan"
                });
            }

            // Melakukan join antara tabel Employee, Education, dan University
            var employeeDetails = from emp in employees
                                  join edu in educations on emp.Guid equals edu.Guid
                                  join uni in universities on edu.UniversityGuid equals uni.Guid
                                  select new EmployeeDetailDto
                                  {
                                      Guid = emp.Guid,
                                      Nik = emp.Nik,
                                      FullName = emp.FirstName + emp.LastName,
                                      BirthDate = emp.BirthDate,
                                      Gender = emp.Gender.ToString(),
                                      HiringDate = emp.HiringDate,
                                      Email = emp.Email,
                                      PhoneNumber = emp.PhoneNumber,
                                      Major = edu.Major,
                                      Degree = edu.Degree,
                                      Gpa = edu.Gpa,
                                      UniveristyName = uni.Name
                                  };

            return Ok(new ResponseOKHandler<IEnumerable<EmployeeDetailDto>>(employeeDetails));
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
                toUpdate.ModifiedDate = entity.ModifiedDate;

                var result = _employeeRepository.Update(toUpdate);

                // Kembalikan pesan sukses dengan respons 200 OK
                // Menggunakan ResponseOKHandler untuk memberikan respons sukses
                var response = new ResponseOKHandler<EmployeeDto>("Data Employee Telah Diperbarui");

                return Ok(response);
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
                var entity2 = _accountRepository.GetByGuid(guid);
                var entity3 = _educationRepository.GetByGuid(guid);
                var entity4 = _bookingRepository.GetByGuid(guid);

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

                // Periksa apakah ada referensi ke karyawan di entitas lain
                bool isReferenced = entity2 != null && entity2.Guid == entity.Guid;
                bool isReferenced2 = entity3 != null && entity3.Guid == entity.Guid;
                bool isReferenced3 = entity4 != null && entity4.EmployeeGuid == entity.Guid;

                if (isReferenced || isReferenced2 || isReferenced3)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Tidak dapat menghapus employee karena masih digunakan oleh entitas lain !",                        
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
