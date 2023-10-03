namespace API.Contracts;

// Antarmuka ini mendefinisikan operasi dasar untuk repositori umum.
// TEntity adalah tipe entitas yang akan digunakan dalam repositori.
public interface IGeneralRepository<TEntity> where TEntity : class
{
    // Mengambil semua entitas dari repositori dan mengembalikannya sebagai IEnumerable<TEntity>.
    IEnumerable<TEntity> GetAll();

    // Mengambil satu entitas berdasarkan Guid yang diberikan.
    // Mengembalikan entitas yang sesuai atau null jika tidak ditemukan.
    TEntity? GetByGuid(Guid guid);

    // Membuat entitas baru dalam repositori.
    // Mengembalikan entitas yang baru dibuat atau null jika gagal membuatnya.
    TEntity? Create(TEntity entity);

    // Memperbarui entitas yang ada dalam repositori.
    // Mengembalikan true jika pembaruan berhasil atau false jika gagal.
    bool Update(TEntity entity);

    // Menghapus entitas dari repositori.
    // Mengembalikan true jika penghapusan berhasil atau false jika gagal.
    bool Delete(TEntity entity);
}