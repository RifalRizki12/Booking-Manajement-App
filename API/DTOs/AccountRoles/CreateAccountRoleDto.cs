using API.Models; // Mengimpor namespace Models

namespace API.DTOs.AccountRoles
{
    public class CreateAccountRoleDto
    {
        // Properti-properti yang akan digunakan dalam kelas CreateAccountRoleDto
        public Guid AccountGuid { get; set; }
        public Guid RoleGuid { get; set; }

        // Konversi Implisit (Implicit Conversion):
        // Metode ini akan mengonversi objek CreateAccountRoleDto menjadi objek AccountRole secara implisit jika diperlukan.
        public static implicit operator AccountRole(CreateAccountRoleDto dto)
        {
            // Membuat objek AccountRole baru dan mengisi propertinya dengan nilai dari objek CreateAccountRoleDto
            return new AccountRole
            {
                AccountGuid = dto.AccountGuid,
                RoleGuid = dto.RoleGuid,
                CreatedDate = DateTime.Now, // Mengatur CreatedDate dengan waktu saat ini
                ModifiedDate = DateTime.Now // Mengatur ModifiedDate dengan waktu saat ini
            };
        }
    }
}
