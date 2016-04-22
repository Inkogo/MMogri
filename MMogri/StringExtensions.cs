using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MMogri
{
    public static class StringExtensions
    {
        public static Guid ToGuid(this string value)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
            return new Guid(data);
        }

        public static string Random(this string s)
        {
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            char ch;
            Random random = new Random();
            for (int i = 0; i < s.Length; i++)
            {
                ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static bool IsEmailAdress(this string s)
        {
            var addr = new System.Net.Mail.MailAddress(s);
            return addr.Address == s;
        }

        public static string NormalizePath(this string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }
    }
}
