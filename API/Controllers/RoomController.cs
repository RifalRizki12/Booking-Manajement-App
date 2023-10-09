using API.Contracts;
using API.DTOs.Employees;
using API.DTOs.Rooms;
using API.Models;
using API.Repositories;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public RoomController(IRoomRepository roomRepository, IBookingRepository bookingRepository, IEmployeeRepository employeeRepository)
        {
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
            _employeeRepository = employeeRepository;
        }

        // Endpoint untuk mendapatkan ruangan yang sedang digunakan hari ini
        [HttpGet("today")]
        public IActionResult GetRoomsInUseToday()
        {
            // Mengambil daftar booking, ruangan, dan karyawan dari repositori
            var booking = _bookingRepository.GetAll();
            var room = _roomRepository.GetAll();
            var employees = _employeeRepository.GetAll();

            // Mendapatkan tanggal hari ini
            DateTime today = DateTime.Now.Date;

            // Memeriksa apakah ada data booking atau ruangan yang tersedia
            if (!(booking.Any() && room.Any()))
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Data Booking atau Room Tidak Ditemukan"
                });
            }

            // Melakukan join antara tabel booking, room, dan employees untuk mendapatkan ruangan yang digunakan hari ini
            var roomsInUseToday = (from bo in booking
                                   join ro in room on bo.RoomGuid equals ro.Guid
                                   join emp in employees on bo.EmployeeGuid equals emp.Guid // Join dengan tabel Employee
                                   where bo.StartDate.Date <= today && today <= bo.EndDate.Date
                                   select new RoomUsageDto
                                   {
                                       BookingGuid = bo.Guid,
                                       Status = bo.Status,
                                       RoomName = ro.Name,
                                       Floor = ro.Floor,
                                       BookedBy = $"{emp.FirstName} {emp.LastName}" // Menggunakan FirstName dan LastName dari tabel Employee
                                   }).ToList();

            // Memeriksa apakah ada ruangan yang digunakan hari ini
            if (!roomsInUseToday.Any())
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Tidak ada ruangan yang digunakan hari ini"
                });
            }

            // Mengembalikan daftar ruangan yang digunakan hari ini dalam respons OK
            return Ok(new ResponseOKHandler<IEnumerable<RoomUsageDto>>(roomsInUseToday));
        }

        // Endpoint untuk mendapatkan ruangan yang sedang tidak digunakan hari ini
        [HttpGet("available-rooms")]
        public IActionResult GetAvailableRooms()
        {
            try
            {
                // Mengambil semua data dari tabel Room
                var rooms = _roomRepository.GetAll();

                // Mengambil semua data dari tabel Booking
                var bookings = _bookingRepository.GetAll();

                // Mendapatkan tanggal hari ini
                DateTime today = DateTime.Now.Date;

                // Mendapatkan daftar RoomGuid yang sedang digunakan pada hari ini
                var usedRoomGuids = bookings
                    .Where(bo => bo.StartDate.Date <= today && today <= bo.EndDate.Date)
                    .Select(bo => bo.RoomGuid)
                    .Distinct()
                    .ToList();

                // Memilih ruangan yang tidak ada dalam daftar usedRoomGuids sebagai ruangan yang tersedia
                var availableRooms = rooms
                    .Where(ro => !usedRoomGuids.Contains(ro.Guid))
                    .Select(ro => new RoomDto
                    {
                        Guid = ro.Guid,
                        Name = ro.Name,
                        Floor = ro.Floor,
                        Capacity = ro.Capacity
                    })
                    .ToList();

                // Jika tidak ada ruangan yang tersedia, kirim respons NotFound
                if (!availableRooms.Any())
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Tidak ada ruangan yang tersedia hari ini"
                    });
                }

                // Mengembalikan daftar ruangan yang tersedia dalam respons OK
                return Ok(new ResponseOKHandler<IEnumerable<RoomDto>>(availableRooms));
            }
            catch (ExceptionHandler ex)
            {
                // Jika terjadi pengecualian saat mengambil data, akan mengembalikan respons kesalahan dengan pesan pengecualian.
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to retrieve available rooms",
                    Error = ex.Message
                });
            }
        }


        // GET api/room
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                // Mengambil semua data ruangan dari repositori.
                var result = _roomRepository.GetAll();

                // Jika tidak ada data yang ditemukan, akan mengembalikan respons "Data Not Found" dengan status "Not Found".
                if (!result.Any())
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Data Not Found"
                    });
                }

                // Data ruangan yang ditemukan akan dikonversi menjadi objek RoomDto dan dikembalikan dalam respons OK.
                var data = result.Select(x => (RoomDto)x);

                return Ok(new ResponseOKHandler<IEnumerable<RoomDto>>(data));
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

        // GET api/room/{guid}
        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            try
            {
                // Mengambil data ruangan berdasarkan GUID yang diberikan dari repositori.
                var result = _roomRepository.GetByGuid(guid);

                // Jika data ruangan dengan GUID yang diberikan tidak ditemukan, akan mengembalikan respons "Id Not Found" dengan status "Not Found".
                if (result is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Data Not Found"
                    });
                }

                // Jika data ruangan ditemukan, akan mengonversinya ke dalam bentuk RoomDto sebelum mengembalikan respons OK.
                return Ok(new ResponseOKHandler<RoomDto>((RoomDto)result));
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

        // POST api/room
        [HttpPost]
        public IActionResult Create(CreateRoomDto roomDto)
        {
            try
            {
                // Menerima data baru untuk ruangan dalam bentuk CreateRoomDto.

                // Mencoba untuk membuat data ruangan baru menggunakan data yang diterima.
                var result = _roomRepository.Create(roomDto);

                // Jika berhasil, akan mengembalikan respons OK dengan data ruangan yang baru dalam bentuk RoomDto.
                if (result is null)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Failed to create data"
                    });
                }

                return Ok(new ResponseOKHandler<RoomDto>((RoomDto)result));
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

        // PUT api/room
        [HttpPut]
        public IActionResult Update(RoomDto roomDto)
        {
            try
            {
                // Menerima data perubahan untuk ruangan dalam bentuk RoomDto.

                // Mencari data ruangan yang ada berdasarkan GUID yang ada dalam RoomDto.
                var entity = _roomRepository.GetByGuid(roomDto.Guid);

                // Jika data ruangan yang ada tidak ditemukan, akan mengembalikan respons "Id Not Found" dengan status "Not Found".
                if (entity is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Data Not Found"
                    });
                }

                // Mengupdate data ruangan yang ada dengan data yang ada dalam RoomDto.
                Room toUpdate = roomDto;
                toUpdate.CreatedDate = entity.CreatedDate;

                // Jika berhasil mengupdate, akan mengembalikan respons "Data Updated".
                var result = _roomRepository.Update(toUpdate);

                // Jika gagal, akan mengembalikan respons "Failed to update data" dengan status "Bad Request".
                if (!result)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Failed to update data"
                    });
                }

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

        // DELETE api/room/{guid}
        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            try
            {
                // Menerima GUID ruangan yang ingin dihapus.

                // Mencari data ruangan yang ada berdasarkan GUID yang diberikan.
                var existingRoom = _roomRepository.GetByGuid(guid);

                // Jika data ruangan tidak ditemukan, akan mengembalikan respons "Room not found" dengan status "Not Found".
                if (existingRoom is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Room not found"
                    });
                }

                // Mencoba untuk menghapus data ruangan dari repositori.
                var deleted = _roomRepository.Delete(existingRoom);

                // Jika berhasil menghapus, akan mengembalikan respons tanpa konten (204 No Content) untuk menunjukkan penghapusan berhasil.
                if (!deleted)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Failed to delete room"
                    });
                }

                return NoContent(); // Kode status 204 No Content untuk sukses penghapusan tanpa respons.
            }
            catch (ExceptionHandler ex)
            {
                // Jika terjadi pengecualian saat menghapus data, akan mengembalikan respons kesalahan dengan pesan pengecualian.
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to delete room",
                    Error = ex.Message
                });
            }
        }
    }
}
