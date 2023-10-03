using API.Models; // Mengimpor namespace yang diperlukan.

namespace API.DTOs.Educations
{
    public class EducationDto
    {
        public Guid Guid { get; set; } // Properti yang menyimpan GUID (identifikasi unik) dari entitas Education.
        public string Major { get; set; } 
        public string Degree { get; set; } 
        public float Gpa { get; set; } 
        public Guid UniversityGuid { get; set; } // Properti yang menyimpan GUID universitas terkait.

        // Operator eksplisit untuk mengonversi dari entitas Education ke EducationDto.
        public static explicit operator EducationDto(Education education)
        {
            return new EducationDto
            {
                Guid = education.Guid, // Mengisi properti Guid dengan nilai dari entitas Education.
                Major = education.Major, 
                Degree = education.Degree, 
                Gpa = education.Gpa, 
                UniversityGuid = education.UniversityGuid
            };
        }

        // Operator implisit untuk mengonversi dari EducationDto ke entitas Education.
        public static implicit operator Education(EducationDto educationDto)
        {
            return new Education
            {
                Guid = educationDto.Guid, // Mengisi properti Guid dengan nilai dari EducationDto.
                Major = educationDto.Major, 
                Degree = educationDto.Degree, 
                Gpa = educationDto.Gpa, 
                UniversityGuid = educationDto.UniversityGuid, 
                ModifiedDate = DateTime.Now 
            };
        }
    }
}
