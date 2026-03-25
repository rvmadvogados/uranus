using System.Security.Cryptography;
using System.Text;

namespace Uranus.AuthApi.Helpers
{
    public static class Md5Helper
    {
        public static string GerarHashMd5(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        public static bool VerificarHashMd5(string input, string hash)
        {
            string hashInput = GerarHashMd5(input);
            return StringComparer.OrdinalIgnoreCase.Compare(hashInput, hash) == 0;
        }
    }
}