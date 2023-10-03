using API.DTOs.Employees; 
using API.Models; 

namespace API.DTOs.AccountRoles
{
    public class AccountRoleDto
    {
        // Properti-properti yang akan digunakan dalam kelas AccountRoleDto
        public Guid Guid { get; set; }
        public Guid AccountGuid { get; set; }
        public Guid RoleGuid { get; set; }

        // Konversi Eksplisit (Explicit Conversion):
        // Metode ini akan mengonversi objek AccountRole menjadi objek AccountRoleDto secara eksplisit jika diperlukan.
        public static explicit operator AccountRoleDto(AccountRole dto)
        {
            // Membuat objek AccountRoleDto baru dan mengisi propertinya dengan nilai dari objek AccountRole
            return new AccountRoleDto
            {
                Guid = dto.Guid,
                AccountGuid = dto.AccountGuid,
                RoleGuid = dto.RoleGuid
            };
        }

        // Konversi Implisit (Implicit Conversion):
        // Metode ini akan mengonversi objek AccountRoleDto menjadi objek AccountRole secara implisit jika diperlukan.
        public static implicit operator AccountRole(AccountRoleDto dto)
        {
            // Membuat objek AccountRole baru dan mengisi propertinya dengan nilai dari objek AccountRoleDto
            return new AccountRole
            {
                Guid = dto.Guid,
                AccountGuid = dto.AccountGuid,
                RoleGuid = dto.RoleGuid,
                ModifiedDate = DateTime.Now // Mengatur ModifiedDate dengan waktu saat ini
            };
        }
    }
}
