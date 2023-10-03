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

        // Operator implisit yang mengubah CreateAccountDto menjadi Account
        public static implicit operator Account(CreateAccountDto createAccountDto)
        {
            // Membuat dan menginisialisasi objek Account baru dengan nilai dari CreateAccountDto
            return new Account
            {
                Guid = createAccountDto.Guid,
                Password = createAccountDto.Password,
                Otp = createAccountDto.Otp,
                IsUsed = createAccountDto.IsUsed,

                // Mengatur nilai ExpiredTime, CreatedDate, dan ModifiedDate dengan waktu saat ini
                ExpiredTime = DateTime.Now,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }
    }
}
