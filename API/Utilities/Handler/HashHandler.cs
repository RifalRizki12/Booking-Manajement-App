namespace API.Utilities.Handler
{
    public class HashHandler
    {
        // Menghasilkan salt acak dengan panjang 12 karakter (default 11).
        private static string GetRandomSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(12); // default 11
        }

        // Meng-hash sebuah kata sandi dengan salt acak.
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, GetRandomSalt());
        }

        // Memverifikasi kata sandi terhadap hashed password yang ada.
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }

}
