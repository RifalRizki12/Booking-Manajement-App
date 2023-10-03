using API.Models;

namespace API.DTOs.Universites
{
    public class CreateUniversityDto
    {
        public string Code { get; set; } // Properti Code digunakan untuk menyimpan kode universitas yang akan dibuat.
        public string Name { get; set; } // Properti Name digunakan untuk menyimpan nama universitas yang akan dibuat.

        // Konversi Implisit (Implicit Conversion):
        // Metode ini akan mengonversi objek CreateUniversityDto ke objek University secara implisit jika diperlukan.
        public static implicit operator University(CreateUniversityDto createUniversityDto)
        {
            return new University
            {
                Code = createUniversityDto.Code,        // Mengambil nilai Code dari objek CreateUniversityDto.
                Name = createUniversityDto.Name,        // Mengambil nilai Name dari objek CreateUniversityDto.
                CreatedDate = DateTime.Now,            // Menambahkan nilai CreatedDate saat mengonversi.
                ModifiedDate = DateTime.Now            // Menambahkan nilai ModifiedDate saat mengonversi.
            };
        }
    }
}
