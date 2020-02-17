using System;
namespace UsersAPI.UtilClasses
{
    public static class PasswordScore
    {
        public static int BLANK = 0;
        public static int WEAK = 1;
        public static int VERY_WEAK = 2;
        public static int MEDIUM = 3;
        public static int STRONG = 4;
        public static int VERY_STRONG = 5;

        public static string PASSWORD_MESSAGE = "In order to set password, please ensure:Minimum 12 characters in length, Contain Uppercase, Contain Lowercase, Contain Number, Contain Symbol";
    }
}
