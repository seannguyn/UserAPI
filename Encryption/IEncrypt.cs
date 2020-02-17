using System;
namespace UsersAPI.Encryption
{
    public interface IEncrypt
    {
        string encryptPassword(string rawPassword);
    }
}
