using System;
using BCrypt.Net;

namespace UsersAPI.Encryption
{
    public class BCrypt_Encrypt : IEncrypt
    {
        public BCrypt_Encrypt()
        {
        }

        public string encryptPassword(string rawPassword)
        {
            string mySalt = BCrypt.Net.BCrypt.GenerateSalt();
            string myHash = BCrypt.Net.BCrypt.HashPassword(rawPassword, mySalt);

            return myHash;
        }
    }
}
