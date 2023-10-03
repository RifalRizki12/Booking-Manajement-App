using API.Data;

namespace API.Utilities.Handler
{
    public class GenerateHandler
    {
        public static string generateNik(string lastNik)
        {
            if (string.IsNullOrEmpty(lastNik))
            {
                return "111111";
            }

            long nextNik = long.Parse(lastNik) + 1;
            return nextNik.ToString();
        }
    }
}
