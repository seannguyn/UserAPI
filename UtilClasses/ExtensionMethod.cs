using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace UsersAPI.UtilClasses
{
    public static class ExtensionMethod
    {
        public static int checkPasswordStrength(this string password)
        {
            int score = 0;

            if (password.Length < 1)
                return PasswordScore.BLANK;
            if (password.Length < 4)
                return PasswordScore.VERY_WEAK;

            if (password.Length >= 8)
                score++;
            if (password.Length >= 12)
                score++;
            if (password.Any(char.IsNumber))
                score++;
            if (password.Any(char.IsUpper) || password.Any(char.IsLower))
                score++;
            if (Regex.Match(password, @"/.[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]/", RegexOptions.ECMAScript).Success)
                score++;
            return score;
        }

        public static IEnumerable<T> WhereDynamic<T>(this IEnumerable<T> source, string columnName, string propertyValue) where T : class
        {
            List<T> result;
            try
            {
                result = source.Where(m => { return m.GetType().GetProperty(columnName).GetValue(m, null).ToString() == propertyValue; }).Select(m => m).ToList();
            }catch(Exception)
            {
                return source;
            }
            
            return result;
        }
    }
}
