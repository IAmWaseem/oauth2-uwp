using System;

namespace OAuth2Manager.Extensions
{
    public static class UriExtensions
    {
        public static Uri ProcessIEUrlErrors(this Uri current)
        {
            if (current.Authority.Equals("ieframe.dll", StringComparison.CurrentCultureIgnoreCase))
            {
                return new Uri(current.Fragment.Substring(1));
            }
            return current;
        }
    }
}
