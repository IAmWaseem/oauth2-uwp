using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OAuth2Manager.Extensions
{
    public static class StringExtensions
    {
        public static string UrlDecode(this string stringToDecode)
        {
            return Uri.UnescapeDataString(stringToDecode.Replace("+", " "))
                .Replace("%21", "!")
                .Replace("%2A", "*")
                .Replace("%27", "'")
                .Replace("%28", "(")
                .Replace("%29", ")");
        }

        public static string ToString<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join(separator, source);
        }

        public static string UrlEncode(this string stringToEscape)
        {
            return Uri.EscapeDataString(stringToEscape)
                .Replace("!", "%21")
                .Replace("*", "%2A")
                .Replace("'", "%27")
                .Replace("(", "%28")
                .Replace(")", "%29");
        }

        public static bool IsJson(this string data)
        {
            var regex = new Regex("\\{(((\\s)*(\\\"|\\\')[^,:\\\'\\\"]+(\\\"|\\\')(\\s)*\\:(\\s)*(\\\"|\\\')?[^,:\\\'\\\"]+(\\\"|\\\')?(\\s)*),)*((\\s)*(\\\"|\\\')[^,:\\\'\\\"]+(\\\"|\\\')(\\s)*\\:(\\s)*(\\\"|\\\')?[^,:\\\'\\\"]+(\\\"|\\\')?(\\s)*)\\}");
            return regex.IsMatch(data);
        }
    }
}
