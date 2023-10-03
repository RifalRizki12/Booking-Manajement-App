using API.Models;

namespace API.DTOs.Accounts
{
    public class CreateAccountDto
    {
        // Properti untuk menyimpan GUID akun
        public Guid Guid { get; set; }

        public string Password { get; set; }

        public int Otp { get; set; }

        // Properti untuk menyimpan informasi apakah akun telah digunakan atau belum
        public bool IsUsed { get; set; }
        public DateTime ExpiredTime { get; set; }

        // Operator implisit yang mengubah CreateAccountDto menjadi Account
        public static implicit operator Account(CreateAccountDto dto)
        {
            // Membuat dan menginisialisasi objek Account baru dengan nilai dari CreateAccountDto
            return new Account
            {
                Guid = dto.Guid,
                Password = dto.Password,
                Otp = dto.Otp,
                IsUsed = dto.IsUsed,
                ExpiredTime = dto.ExpiredTime,

                // Mengatur nilai ExpiredTime, CreatedDate, dan ModifiedDate dengan waktu saat ini
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }
    }
}
