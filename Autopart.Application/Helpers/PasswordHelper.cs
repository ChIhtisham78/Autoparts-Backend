using System.Security.Cryptography;
using System.Text;

namespace Autopart.Application.Helpers
{
    public class PasswordHelper : IPasswordHelper
    {
        public string CreatePasswordHash(string salt, string password)
        {



            string saltAndPwd = String.Concat(password, salt);
            var sha1 = SHA1.Create();
            var step1 = Encoding.UTF8.GetBytes(saltAndPwd);
            var step2 = sha1.ComputeHash(step1);
            var step3 = BitConverter.ToString(step2).Replace("-", string.Empty);
            return step3.ToString();
        }

        public string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            var rng = RandomNumberGenerator.Create();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            string salt = Convert.ToBase64String(buff);
            return salt;
        }
    }
}
