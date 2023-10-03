using API.Models;

namespace API.DTOs.Univers
{
    public class UniversityDto
    {
        public Guid Guid { get; set; } // Properti Guid digunakan untuk menyimpan ID universitas.
        public string Code { get; set; } // Properti Code digunakan untuk menyimpan kode universitas.
        public string Name { get; set; } // Properti Name digunakan untuk menyimpan nama universitas.

        // Konversi Eksplisit (Explicit Conversion):
        // Metode ini akan mengonversi objek University ke UniversityDto secara eksplisit jika diperlukan.
        public static explicit operator UniversityDto(University university)
        {
            return new UniversityDto
            {
                Guid = university.Guid,   // Mengambil nilai Guid dari objek University.
                Code = university.Code,   // Mengambil nilai Code dari objek University.
                Name = university.Name    // Mengambil nilai Name dari objek University.
            };
        }

        // Konversi Implisit (Implicit Conversion):
        // Metode ini akan mengonversi objek UniversityDto ke University secara implisit jika diperlukan.
        public static implicit operator University(UniversityDto universityDto)
        {
            return new University
            {
                Guid = universityDto.Guid,        // Mengambil nilai Guid dari objek UniversityDto.
                Code = universityDto.Code,        // Mengambil nilai Code dari objek UniversityDto.
                Name = universityDto.Name,        // Mengambil nilai Name dari objek UniversityDto.
                ModifiedDate = DateTime.Now       // Menambahkan nilai ModifiedDate saat mengonversi.
            };
        }
    }
}
