using API.Contracts;  // Mengimpor namespace API.Contracts yang diperlukan.
using API.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class GeneralRepository<TEntity> : IGeneralRepository<TEntity> where TEntity : class
    {
        protected readonly BookingManagementDbContext _context;  // Mendefinisikan instance dari DbContext untuk mengakses database.

        public GeneralRepository(BookingManagementDbContext context)
        {
            _context = context;  // Menginisialisasi DbContext melalui konstruktor.
        }

        // Mendapatkan semua entitas TEntity dari database dan mengembalikannya dalam bentuk IEnumerable.
        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().ToList();
        }

        // Mencari entitas TEntity berdasarkan GUID yang diberikan.
        public TEntity? GetByGuid(Guid guid)
        {
            // Menggunakan metode Find() dari DbContext untuk mencari entitas berdasarkan GUID.
            var entity = _context.Set<TEntity>().Find(guid);

            // Membersihkan ChangeTracker untuk menghindari pemantauan entitas yang tidak perlu.
            _context.ChangeTracker.Clear();

            return entity;  // Mengembalikan entitas yang ditemukan atau null jika tidak ada.
        }

        // Menambahkan entitas baru TEntity ke dalam database.
        public TEntity? Create(TEntity entity)
        {
            try
            {
                _context.Set<TEntity>().Add(entity);  // Menambahkan entitas ke DbSet yang sesuai.
                _context.SaveChanges();  // Menyimpan perubahan ke dalam database.
                return entity;  // Mengembalikan entitas yang berhasil ditambahkan.
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;
                if (innerException is SqlException sqlException)
                {
                    // Cek apakah ada indeks unik yang dilanggar
                    if (sqlException.Number == 2601)
                    {
                        // Pengecualian ini disebabkan oleh duplikasi data pada indeks unik
                        // Anda dapat mengekstrak informasi yang relevan dari innerException
                        // dan memberikan pesan kesalahan yang sesuai
                        if (sqlException.Message.Contains("IX_tb_m_employees_email"))
                        {
                            throw new Exception("Email sudah digunakan.", ex);
                        }
                        else if (sqlException.Message.Contains("IX_tb_m_employees_number"))
                        {
                            throw new Exception("Number sudah digunakan.", ex);
                        }
                        else if (sqlException.Message.Contains("IX_tb_m_employees_nik"))
                        {
                            throw new Exception("NIK sudah digunakan.", ex);
                        }
                    }
                }

                // Tangani pengecualian lainnya dan kembalikan null jika terjadi kesalahan selama penambahan.
                return null;
            }
        }


        // Memperbarui entitas TEntity yang ada dalam database.
        public bool Update(TEntity entity)
        {
            try
            {
                _context.Set<TEntity>().Update(entity);  // Memperbarui entitas dalam DbSet yang sesuai.
                _context.SaveChanges();  // Menyimpan perubahan ke dalam database.
                return true;  // Mengembalikan true jika pembaruan berhasil.
            }
            catch
            {
                return false;  // Mengembalikan false jika terjadi kesalahan selama pembaruan.
            }
        }

        // Menghapus entitas TEntity dari database.
        public bool Delete(TEntity entity)
        {
            try
            {
                _context.Set<TEntity>().Remove(entity);  // Menghapus entitas dari DbSet yang sesuai.
                _context.SaveChanges();  // Menyimpan perubahan ke dalam database.
                return true;  // Mengembalikan true jika penghapusan berhasil.
            }
            catch
            {
                return false;  // Mengembalikan false jika terjadi kesalahan selama penghapusan.
            }
        }
    }
}
