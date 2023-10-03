using API.Contracts;
using API.DTOs.Rooms;
using API.DTOs.Rooms;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Mengambil semua data ruangan dari repositori.
            var result = _roomRepository.GetAll();

            // Jika tidak ada data yang ditemukan, akan mengembalikan respons "Data Not Found" dengan status "Not Found".
            if (!result.Any())
            {
                return NotFound("Data Not Found");
            }

            // Data ruangan yang ditemukan akan dikonversi menjadi objek RoomDto dan dikembalikan dalam respons OK.
            var data = result.Select(x => (RoomDto)x);

            return Ok(data);
        }

        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            // Mengambil data ruangan berdasarkan GUID yang diberikan dari repositori.
            var result = _roomRepository.GetByGuid(guid);

            // Jika data ruangan dengan GUID yang diberikan tidak ditemukan, akan mengembalikan respons "Id Not Found" dengan status "Not Found".
            if (result is null)
            {
                return NotFound("Id Not Found");
            }

            // Jika data ruangan ditemukan, akan mengonversinya ke dalam bentuk RoomDto sebelum mengembalikan respons OK.
            return Ok((RoomDto)result);
        }

        [HttpPost]
        public IActionResult Create(CreateRoomDto roomDto)
        {
            // Menerima data baru untuk ruangan dalam bentuk CreateRoomDto.

            // Mencoba untuk membuat data ruangan baru menggunakan data yang diterima.
            var result = _roomRepository.Create(roomDto);

            // Jika berhasil, akan mengembalikan respons OK dengan data ruangan yang baru dalam bentuk RoomDto.
            if (result is null)
            {
                return BadRequest("Failed to create data");
            }

            return Ok((RoomDto)result);
        }

        [HttpPut]
        public IActionResult Update(RoomDto roomDto)
        {
            // Menerima data perubahan untuk ruangan dalam bentuk RoomDto.

            // Mencari data ruangan yang ada berdasarkan GUID yang ada dalam RoomDto.
            var entity = _roomRepository.GetByGuid(roomDto.Guid);

            // Jika data ruangan yang ada tidak ditemukan, akan mengembalikan respons "Id Not Found" dengan status "Not Found".
            if (entity is null)
            {
                return NotFound("Id Not Found");
            }

            // Mengupdate data ruangan yang ada dengan data yang ada dalam RoomDto.
            Room toUpdate = roomDto;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Jika berhasil mengupdate, akan mengembalikan respons "Data Updated".
            var result = _roomRepository.Update(toUpdate);

            // Jika gagal, akan mengembalikan respons "Failed to update data" dengan status "Bad Request".
            if (!result)
            {
                return BadRequest("Failed to update data");
            }

            return Ok("Data Updated");
        }

        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            // Menerima GUID ruangan yang ingin dihapus.

            // Mencari data ruangan yang ada berdasarkan GUID yang diberikan.
            var existingRoom = _roomRepository.GetByGuid(guid);

            // Jika data ruangan tidak ditemukan, akan mengembalikan respons "Room not found" dengan status "Not Found".
            if (existingRoom is null)
            {
                return NotFound("Room not found");
            }

            // Mencoba untuk menghapus data ruangan dari repositori.
            var deleted = _roomRepository.Delete(existingRoom);

            // Jika berhasil menghapus, akan mengembalikan respons tanpa konten (204 No Content) untuk menunjukkan penghapusan berhasil.
            if (!deleted)
            {
                return BadRequest("Failed to delete room");
            }

            return NoContent(); // Kode status 204 No Content untuk sukses penghapusan tanpa respons.
        }
    }
}
