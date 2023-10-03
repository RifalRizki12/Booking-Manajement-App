using API.Models;

namespace API.DTOs.Roles
{
    public class RoleDto
    {
        public Guid Guid { get; set; } // Properti yang menyimpan GUID entitas Role.
        public string Name { get; set; } // Properti yang menyimpan nama peran (role).

        // Operator eksplisit untuk mengonversi Role ke RoleDto.
        public static explicit operator RoleDto(Role role)
        {
            return new RoleDto
            {
                Guid = role.Guid, // Mengisi properti Guid dengan nilai dari entitas Role.
                Name = role.Name  // Mengisi properti Name dengan nilai dari entitas Role.
            };
        }

        // Operator implisit untuk mengonversi RoleDto ke Role.
        public static implicit operator Role(RoleDto roleDto)
        {
            return new Role
            {
                Guid = roleDto.Guid,       // Mengisi properti Guid dengan nilai dari RoleDto.
                Name = roleDto.Name,       // Mengisi properti Name dengan nilai dari RoleDto.
                ModifiedDate = DateTime.Now // Mengisi properti ModifiedDate dengan tanggal dan waktu saat ini.
            };
        }
    }
}
