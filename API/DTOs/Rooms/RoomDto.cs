using API.DTOs.Universites;
using API.Models;

namespace API.DTOs.Rooms
{
    public class RoomDto
    {
        public Guid Guid { get; set; }     
        public string Name { get; set; }   
        public int Floor { get; set; }     
        public int Capacity { get; set; }  

        // Konversi Eksplisit (Explicit Conversion):
        // Metode ini akan mengonversi objek Room ke objek RoomDto secara eksplisit jika diperlukan.
        public static explicit operator RoomDto(Room room)
        {
            return new RoomDto
            {
                Guid = room.Guid,          // Mengambil nilai Guid dari objek Room.
                Name = room.Name,          // Mengambil nilai Name dari objek Room.
                Floor = room.Floor,        // Mengambil nilai Floor dari objek Room.
                Capacity = room.Capacity   // Mengambil nilai Capacity dari objek Room.
            };
        }

        // Konversi Implisit (Implicit Conversion):
        // Metode ini akan mengonversi objek RoomDto ke objek Room secara implisit jika diperlukan.
        // Metode ini juga melakukan pembaruan data pada properti ModifiedDate.
        public static implicit operator Room(RoomDto roomDto)
        {
            return new Room
            {
                Guid = roomDto.Guid,                // Mengambil nilai Guid dari objek RoomDto.
                Name = roomDto.Name,                // Mengambil nilai Name dari objek RoomDto.
                Floor = roomDto.Floor,              // Mengambil nilai Floor dari objek RoomDto.
                Capacity = roomDto.Capacity,        // Mengambil nilai Capacity dari objek RoomDto.
                ModifiedDate = DateTime.Now        // Menambahkan nilai ModifiedDate saat mengonversi.
            };
        }
    }
}
