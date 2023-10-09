using API.Contracts;
using API.DTOs.Bookings;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using API.Utilities.Handler;
using System.Net;
using API.Repositories;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public BookingController(IBookingRepository bookingRepository, IRoomRepository roomRepository, IEmployeeRepository employeeRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _employeeRepository = employeeRepository;
        }

        [HttpGet("details")]
        public IActionResult GetAllBookingDetails()
        {
            // Mengambil daftar booking, karyawan yang berhubungan dengan booking, dan ruangan dari repositori
            var bookings = _bookingRepository.GetAll();
            var bookingEmployees = _employeeRepository.GetAll(); // Tabel Employee yang berelasi dengan Booking
            var rooms = _roomRepository.GetAll();

            // Menggabungkan tabel booking, karyawan, dan ruangan untuk mendapatkan detail booking
            var bookingDetails = (from bo in bookings
                                  join emp in bookingEmployees on bo.EmployeeGuid equals emp.Guid
                                  join ro in rooms on bo.RoomGuid equals ro.Guid
                                  select new BookingDetailDto
                                  {
                                      Guid = bo.Guid,
                                      BookedNIK = emp.Nik,
                                      BookedBy = $"{emp.FirstName} {emp.LastName}",
                                      RoomName = ro.Name,
                                      StartDate = bo.StartDate,
                                      EndDate = bo.EndDate,
                                      Status = bo.Status,
                                      Remarks = bo.Remarks
                                  }).ToList();

            // Memeriksa apakah ada detail booking yang ditemukan
            if (!bookingDetails.Any())
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Tidak ada detail booking yang ditemukan"
                });
            }

            // Mengembalikan daftar detail booking dalam respons OK
            return Ok(new ResponseOKHandler<IEnumerable<BookingDetailDto>>(bookingDetails));
        }


        [HttpGet("details/{guid}", Name = "GetBookingByGuid")]
        public IActionResult GetBookingByGuid(Guid guid)
        {
            // Mengambil booking berdasarkan GUID yang diberikan
            var booking = _bookingRepository.GetByGuid(guid);
            var bookingEmployees = _employeeRepository.GetAll(); // Tabel Employee yang berelasi dengan Booking
            var rooms = _roomRepository.GetAll();

            // Memeriksa apakah booking dengan GUID yang diberikan ditemukan
            if (booking == null)
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Booking dengan GUID yang diberikan tidak ditemukan"
                });
            }

            // Menggabungkan tabel booking, karyawan, dan ruangan untuk mendapatkan detail booking berdasarkan GUID
            var bookingDetail = (from bo in new[] { booking }
                                 join emp in bookingEmployees on bo.EmployeeGuid equals emp.Guid
                                 join ro in rooms on bo.RoomGuid equals ro.Guid
                                 select new BookingDetailDto
                                 {
                                     Guid = bo.Guid,
                                     BookedNIK = emp.Nik,
                                     BookedBy = $"{emp.FirstName} {emp.LastName}",
                                     RoomName = ro.Name,
                                     StartDate = bo.StartDate,
                                     EndDate = bo.EndDate,
                                     Status = bo.Status,
                                     Remarks = bo.Remarks
                                 }).FirstOrDefault();

            // Memeriksa apakah detail booking dengan GUID yang diberikan ditemukan
            if (bookingDetail == null)
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Detail booking dengan GUID yang diberikan tidak ditemukan"
                });
            }

            // Mengembalikan detail booking dalam respons OK
            return Ok(new ResponseOKHandler<BookingDetailDto>(bookingDetail));
        }

        [HttpGet("booking-length")]
        public IActionResult GetBookingLength()
        {
            try
            {
                // Mengambil semua data dari tabel Booking dan Room
                var bookings = _bookingRepository.GetAll();
                var rooms = _roomRepository.GetAll();

                // Mendefinisikan hari-hari yang tidak dihitung (Sabtu dan Minggu)
                var nonWorkingDays = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };

                // Membuat daftar objek RoomBookingLengthDto yang akan menyimpan hasil perhitungan
                var roomBookingLengths = new List<RoomBookingLengthDto>();

                foreach (var room in rooms)
                {
                    // Mengambil semua booking untuk ruangan ini
                    var roomBookings = bookings.Where(bo => bo.RoomGuid == room.Guid);

                    if (roomBookings.Any())
                    {
                        // Menghitung durasi total peminjaman (hari kerja) untuk ruangan ini
                        var totalBookingLengthInHours = 0;

                        foreach (var booking in roomBookings)
                        {
                            var startDate = booking.StartDate;
                            var endDate = booking.EndDate;

                            while (startDate <= endDate)
                            {
                                if (!nonWorkingDays.Contains(startDate.DayOfWeek))
                                {
                                    totalBookingLengthInHours += 1; // Menambahkan 1 jam setiap hari kerja
                                }
                                startDate = startDate.AddHours(1); // Menambahkan 1 jam ke waktu mulai
                            }
                        }

                        // Menghitung jumlah hari
                        var totalBookingLengthInDays = totalBookingLengthInHours / 24;

                        // Menghitung sisa jam
                        var remainingHours = totalBookingLengthInHours % 24;

                        // Menambahkan hasil perhitungan ke daftar
                        roomBookingLengths.Add(new RoomBookingLengthDto
                        {
                            RoomGuid = room.Guid,
                            RoomName = room.Name,
                            BookingLength = $"{totalBookingLengthInDays} Hari {remainingHours} Jam"
                        });
                    }
                }

                // Mengembalikan daftar hasil perhitungan dalam respons OK
                return Ok(new ResponseOKHandler<IEnumerable<RoomBookingLengthDto>>(roomBookingLengths));

            }
            catch (ExceptionHandler ex)
            {
                // Jika terjadi pengecualian saat mengambil data, akan mengembalikan respons kesalahan dengan pesan pengecualian.
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to calculate booking lengths",
                    Error = ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                // Memanggil metode GetAll dari _bookingRepository.
                var result = _bookingRepository.GetAll();

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
                var data = result.Select(x => (BookingDto)x);

                // Mengembalikan data yang ditemukan dalam respons OK.
                return Ok(new ResponseOKHandler<IEnumerable<BookingDto>>(data));
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

        // GET api/booking/{guid}
        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            try
            {
                // Memanggil metode GetByGuid dari _bookingRepository dengan parameter GUID.
                var result = _bookingRepository.GetByGuid(guid);

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
                return Ok(new ResponseOKHandler<BookingDto>((BookingDto)result));
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

        // POST api/booking
        [HttpPost]
        public IActionResult Create(CreateBookingDto bookingDto)
        {
            try
            {
                // Memanggil metode Create dari _bookingRepository dengan parameter DTO.
                var result = _bookingRepository.Create(bookingDto);

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
                return Ok(new ResponseOKHandler<BookingDto>((BookingDto)result));
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

        // PUT api/booking
        [HttpPut]
        public IActionResult Update(BookingDto bookingDto)
        {
            try
            {
                // Memeriksa apakah entitas Booking yang akan diperbarui ada dalam database.
                var entity = _bookingRepository.GetByGuid(bookingDto.Guid);

                if (entity is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Booking not found"
                    });
                }

                // Menyimpan tanggal pembuatan entitas Booking sebelum pembaruan.
                Booking toUpdate = bookingDto;
                toUpdate.CreatedDate = entity.CreatedDate;

                // Memanggil metode Update dari _bookingRepository.
                var result = _bookingRepository.Update(toUpdate);

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

        // DELETE api/booking/{guid}
        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            try
            {
                // Memanggil metode GetByGuid dari _bookingRepository untuk mendapatkan entitas yang akan dihapus.
                var existingBooking = _bookingRepository.GetByGuid(guid);

                // Memeriksa apakah entitas yang akan dihapus ada dalam database.
                if (existingBooking is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Booking not found"
                    });
                }

                // Memanggil metode Delete dari _bookingRepository.
                var deleted = _bookingRepository.Delete(existingBooking);

                // Memeriksa apakah penghapusan data berhasil atau gagal.
                if (!deleted)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Failed to delete booking"
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
                    Message = "Failed to delete booking",
                    Error = ex.Message
                });
            }
        }
    }
}
