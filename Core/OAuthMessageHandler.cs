using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OAuth2Manager.Common;
using OAuth2Manager.Extensions;
using OAuth2Manager.Utils;

namespace OAuth2Manager.Core
{
    public class OAuthMessageHandler : DelegatingHandler
    {
        private readonly string _clientId;
        private string _redirectUrl;
        private readonly IEnumerable<KeyValuePair<string, string>> _parameters;

        public OAuthMessageHandler(string clientId, string redirectUrl,
            IEnumerable<KeyValuePair<string, string>> optionalParameters = null) : base(new HttpClientHandler())
        {
            _clientId = clientId;
            _redirectUrl = redirectUrl ?? OAuthConstants.LOCALHOST;
            _parameters = optionalParameters ?? Enumerable.Empty<KeyValuePair<string, string>>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var parametersToSend = _parameters;
            if (request.Method == HttpMethod.Post)
            {
                if (request.Content is FormUrlEncodedContent)
                {
                    var extraParams = await request.Content.ReadAsStringAsync();
                    var parsedParams = OAuthUtils.ParseQueryString(extraParams);

                    parametersToSend = parametersToSend.Concat(parsedParams);
                }
            }

            var authParams = OAuthUtils.BuildBasicParams(_clientId, parametersToSend);

            if (request.Method == HttpMethod.Post)
            {
                request.Content = new FormUrlEncodedContent(authParams);
            }
            else if (request.Method == HttpMethod.Get)
            {
                var queryData = authParams.Select(p => p.Key + "=" + p.Value).ToString("&");
                string newQuery = request.RequestUri.Query;

                if (string.IsNullOrWhiteSpace(newQuery))
                    newQuery = "?" + queryData;
                else
                    newQuery += "&" + queryData;

                request.RequestUri = new System.Uri(request.RequestUri.OriginalString + newQuery);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
