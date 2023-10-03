using API.Models;
namespace API.DTOs.Roles
{
    public class CreateRoleDto
    {
        public string Name { get; set; } // Properti yang menyimpan nama peran (role) yang akan dibuat.

        // Operator implisit untuk mengonversi CreateRoleDto ke entitas Role.
        public static implicit operator Role(CreateRoleDto createRolesDto)
        {
            return new Role
            {
                Name = createRolesDto.Name, // Mengisi properti Name dengan nilai dari CreateRoleDto.
                CreatedDate = DateTime.Now, // Mengisi properti CreatedDate dengan tanggal dan waktu saat ini.
                ModifiedDate = DateTime.Now // Mengisi properti ModifiedDate dengan tanggal dan waktu saat ini.
            };
        }
    }
}
