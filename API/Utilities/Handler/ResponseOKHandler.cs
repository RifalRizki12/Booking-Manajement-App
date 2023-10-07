using System.Net;

namespace API.Utilities.Handler
{
    public class ResponseOKHandler<TEntity>
    {
        // Properti Code digunakan untuk menyimpan kode status HTTP, dalam hal ini, kode 200 yang sesuai dengan OK.
        public int Code { get; set; }

        // Properti Status digunakan untuk menyimpan status HTTP dalam bentuk teks, yaitu "OK" untuk respons HTTP 200 OK.
        public string Status { get; set; }

        // Properti Message digunakan untuk menyimpan pesan deskriptif yang menjelaskan status respons.
        // Misalnya, "Success to Retrieve Data" untuk respons yang mengambil data dengan sukses.
        public string Message { get; set; }

        // Properti Data adalah generic dan digunakan untuk menyimpan data yang akan dikirimkan sebagai bagian dari respons.
        // Ini bisa berupa entitas, objek, atau tipe data lain yang perlu dikirimkan dalam respons.
        public TEntity? Data { get; set; }

        // Konstruktor pertama digunakan ketika hanya data yang ingin dikirimkan dalam respons.
        public ResponseOKHandler(TEntity? data)
        {
            // Mengatur kode status HTTP ke 200 OK.
            Code = StatusCodes.Status200OK;

            // Mengatur status teks ke "OK".
            Status = HttpStatusCode.OK.ToString();

            // Mengatur pesan respons ke "Success to Retrieve Data".
            Message = "Success to Retrieve Data";

            // Mengatur data yang akan dikirim dalam respons.
            Data = data;
        }

        // Konstruktor kedua digunakan ketika hanya pesan respons yang ingin dikirimkan.
        public ResponseOKHandler(string message)
        {
            // Mengatur kode status HTTP ke 200 OK.
            Code = StatusCodes.Status200OK;

            // Mengatur status teks ke "OK".
            Status = HttpStatusCode.OK.ToString();

            // Mengatur pesan respons sesuai dengan yang diberikan sebagai parameter.
            Message = message;
        }

        public ResponseOKHandler(string message, TEntity data)
        {
            // Mengatur kode status HTTP ke 200 OK.
            Code = StatusCodes.Status200OK;

            // Mengatur status teks ke "OK".
            Status = HttpStatusCode.OK.ToString();

            // Mengatur pesan respons sesuai dengan yang diberikan sebagai parameter.
            Message = message;
            Data = data;
        }
    }
}
