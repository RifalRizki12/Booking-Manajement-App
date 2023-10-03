using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    // Menentukan nama tabel yang sesuai dalam basis data.
    [Table("tb_m_educations")] 
    public class Education : BaseEntity
    {
        [Column("major", TypeName = "nvarchar(100)")] // Menentukan kolom dan tipe datanya.
        public string Major { get; set; }

        [Column("degree", TypeName = "nvarchar(100)")]
        public string Degree { get; set; }

        [Column("gpa")] 
        public float Gpa { get; set; } 

        [Column("university_guid")] 
        public Guid UniversityGuid { get; set; } // Properti GUID untuk mengaitkan dengan universitas (University).

        // Properti navigasi yang menggambarkan hubungan dengan model lain.
        // University adalah properti yang mengacu pada model University, menunjukkan hubungan antara Education dan University.
        public University? University { get; set; }

        // Employee adalah properti yang mengacu pada model Employee, menunjukkan hubungan antara Education dan Employee.
        public Employee? Employee { get; set; }
    }
}
