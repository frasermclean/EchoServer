using System;
using System.Text;

namespace EchoServer
{
    public static class StringExtensions
    {
        public static string ToControlCodeString(this string s)
        {
            var sb = new StringBuilder();

            foreach (char c in s)
            {
                if (char.IsControl(c))
                    sb.AppendFormat("\\x{0:X2}", Convert.ToInt32(c));
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
