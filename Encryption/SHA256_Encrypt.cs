using System;
using System.Text;
using System.Security.Cryptography;

namespace UsersAPI.Encryption
{
    public class SHA256_Encrypt : IEncrypt
    {
        public SHA256_Encrypt()
        {
        }

        public string encryptPassword(string rawPassword)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(rawPassword));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
