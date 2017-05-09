using System.Collections.Generic;
using System.Linq;
using OAuth2Manager.Common;
using OAuth2Manager.Extensions;

namespace OAuth2Manager.Utils
{
    public class OAuthUtils
    {
        public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(string query)
        {
            var queryParams = query.TrimStart('?').Split('&').Where(x => x != "").Select(x =>
            {
                var xs = x.Split('=');
                return new KeyValuePair<string, string>(xs[0].UrlDecode(), xs[1].UrlDecode());
            });
            return queryParams;
        }

        public static IEnumerable<KeyValuePair<string, string>> BuildBasicParams(string clientId,
            IEnumerable<KeyValuePair<string, string>> passedParameters)
        {
            Precondition.NotNullOrEmpty(clientId, nameof(clientId));

            var parameters = new List<KeyValuePair<string, string>>(8)
            {
                new KeyValuePair<string, string>(OAuthConstants.CLIENT_ID, clientId)
            };

            if (passedParameters != null)
            {
                parameters.AddRange(passedParameters);
            }

            return parameters;
        }
    }
}
