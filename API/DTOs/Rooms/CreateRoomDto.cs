using API.Models;

namespace API.DTOs.Rooms
{
    public class CreateRoomDto
    {
        public string Name { get; set; }   
        public int Floor { get; set; }     
        public int Capacity { get; set; }  

        // Konversi Implisit (Implicit Conversion):
        // Metode ini akan mengonversi objek CreateRoomDto ke objek Room secara implisit.
        // Saat melakukan konversi, nilai CreatedDate dan ModifiedDate akan diatur ke DateTime.Now.
        public static implicit operator Room(CreateRoomDto createRoomDto)
        {
            return new Room
            {
                Name = createRoomDto.Name,          // Mengambil nilai Name dari objek CreateRoomDto.
                Floor = createRoomDto.Floor,        // Mengambil nilai Floor dari objek CreateRoomDto.
                Capacity = createRoomDto.Capacity,  // Mengambil nilai Capacity dari objek CreateRoomDto.
                CreatedDate = DateTime.Now,         // Mengatur CreatedDate ke waktu saat ini.
                ModifiedDate = DateTime.Now         // Mengatur ModifiedDate ke waktu saat ini.
            };
        }
    }
}
