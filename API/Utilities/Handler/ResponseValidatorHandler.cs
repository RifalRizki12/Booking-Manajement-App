using System.Net;

namespace API.Utilities.Handler
{
    public class ResponseValidatorHandler
    {
        // Properti untuk kode tanggapan HTTP
        public int Code { get; set; }

        // Properti untuk status tanggapan HTTP
        public string Status { get; set; }

        // Properti untuk pesan tanggapan
        public string Message { get; set; }

        // Properti untuk objek error yang berkaitan dengan validasi
        public object Error { get; set; }

        // Konstruktor kelas ResponseValidatorHandler
        public ResponseValidatorHandler(object error)
        {
            // Mengatur properti-properti tanggapan dalam kasus validasi yang gagal

            // Kode tanggapan HTTP (400 Bad Request)
            Code = StatusCodes.Status400BadRequest;

            // Status tanggapan HTTP (BadRequest)
            Status = HttpStatusCode.BadRequest.ToString();

            // Pesan tanggapan yang mengindikasikan terjadinya kesalahan validasi
            Message = "Validation Error";

            // Objek error yang mungkin berisi detail kesalahan validasi
            Error = error;
        }
    }

}
