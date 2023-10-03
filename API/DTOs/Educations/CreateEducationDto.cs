using API.DTOs.Universites; // Mengimpor namespace yang diperlukan.
using API.Models; 
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs.Educations
{
    public class CreateEducationDto
    {
        public Guid Guid { get; set; } // Properti yang menyimpan GUID (identifikasi unik) dari entitas Education.
        public string Major { get; set; }
        public string Degree { get; set; }
        public float Gpa { get; set; }
        public Guid UniversityGuid { get; set; } // Properti yang menyimpan GUID universitas terkait

        // Operator implisit untuk mengonversi dari CreateEducationDto ke entitas Education.
        public static implicit operator Education(CreateEducationDto createEducationDto)
        {
            return new Education
            {
                Guid = createEducationDto.Guid, // Mengisi properti Guid dengan nilai dari createEducationDto.
                Major = createEducationDto.Major,
                Degree = createEducationDto.Degree,
                Gpa = createEducationDto.Gpa, // Mengisi properti Gpa dengan nilai dari createEducationDto.
                UniversityGuid = createEducationDto.UniversityGuid, // Mengisi properti UniversityGuid dengan nilai dari createEducationDto.
                CreatedDate = DateTime.Now, // Mengisi properti CreatedDate dengan tanggal dan waktu saat ini.
                ModifiedDate = DateTime.Now
            };
        }
    }
}