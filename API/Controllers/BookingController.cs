// Mengimpor namespace yang diperlukan.
using API.Contracts;
using API.DTOs.Bookings;
using API.DTOs.Employees;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

// Mendefinisikan controller dengan atribut ApiController dan route "/api/[controller]".
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        // Mendeklarasikan readonly field _bookingRepository sebagai implementasi IBookingRepository.
        private readonly IBookingRepository _bookingRepository;

        // Konstruktor controller yang menerima IBookingRepository sebagai parameter.
        public BookingController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        // HTTP GET endpoint untuk mengambil semua data Booking.
        [HttpGet]
        public IActionResult GetAll()
        {
            // Memanggil metode GetAll dari _bookingRepository.
            var result = _bookingRepository.GetAll();

            // Memeriksa apakah hasil query tidak mengandung data.
            if (!result.Any())
            {
                return NotFound("Data Not Found");
            }

            // Mengonversi hasil query ke objek DTO (Data Transfer Object) menggunakan Select.
            var data = result.Select(x => (BookingDto)x);

            // Mengembalikan data yang ditemukan dalam respons OK.
            return Ok(data);
        }

        // HTTP GET endpoint untuk mengambil data Booking berdasarkan GUID.
        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            // Memanggil metode GetByGuid dari _bookingRepository dengan parameter GUID.
            var result = _bookingRepository.GetByGuid(guid);

            // Memeriksa apakah hasil query tidak ditemukan (null).
            if (result is null)
            {
                return NotFound("Id Not Found");
            }

            // Mengembalikan data yang ditemukan dalam respons OK.
            return Ok(result);
        }

        // HTTP POST endpoint untuk membuat data Booking baru.
        [HttpPost]
        public IActionResult Create(CreateBookingDto bookingDto)
        {
            // Memanggil metode Create dari _bookingRepository dengan parameter DTO.
            var result = _bookingRepository.Create(bookingDto);

            // Memeriksa apakah penciptaan data berhasil atau gagal.
            if (result is null)
            {
                return BadRequest("Failed to create data");
            }

            // Mengembalikan data yang berhasil dibuat dalam respons OK.
            return Ok((BookingDto)result);
        }

        // HTTP PUT endpoint untuk memperbarui data Booking.
        [HttpPut]
        public IActionResult Update(BookingDto bookingDto)
        {
            // Memeriksa apakah entitas Booking yang akan diperbarui ada dalam database.
            var entity = _bookingRepository.GetByGuid(bookingDto.Guid);

            if (entity is null)
            {
                return NotFound("Id Not Found");
            }

            // Menyalin nilai CreatedDate dari entitas yang ada ke entitas yang akan diperbarui.
            Booking toUpdate = bookingDto;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Memanggil metode Update dari _bookingRepository.
            var result = _bookingRepository.Update(toUpdate);

            // Memeriksa apakah pembaruan data berhasil atau gagal.
            if (!result)
            {
                return BadRequest("Failed to update data");
            }

            // Mengembalikan pesan sukses dalam respons OK.
            return Ok("Data Updated");
        }

        // HTTP DELETE endpoint untuk menghapus data Booking berdasarkan GUID.
        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            // Memanggil metode GetByGuid dari _bookingRepository untuk mendapatkan entitas yang akan dihapus.
            var existingBooking = _bookingRepository.GetByGuid(guid);

            // Memeriksa apakah entitas yang akan dihapus ada dalam database.
            if (existingBooking is null)
            {
                return NotFound("Booking not found");
            }

            // Memanggil metode Delete dari _bookingRepository.
            var deleted = _bookingRepository.Delete(existingBooking);

            // Memeriksa apakah penghapusan data berhasil atau gagal.
            if (!deleted)
            {
                return BadRequest("Failed to delete booking");
            }

            // Mengembalikan kode status 204 (No Content) untuk sukses penghapusan tanpa respons.
            return NoContent();
        }
    }
}
