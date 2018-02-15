// Beware of the .NET HttpClient
// http://www.nimaara.com/2016/11/01/beware-of-the-net-httpclient/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Helpers
{
    public class HttpClientHandler : IHttpHandler
    {
        private readonly HttpClient _client = new HttpClient();

        public HttpResponseMessage Send(HttpRequestMessage requestMessage)
        {
            var servicePoint = ServicePointManager.FindServicePoint(requestMessage.RequestUri);
            servicePoint.ConnectionLeaseTimeout = 60 * 1000; // 1 minute

            var response = _client.SendAsync(requestMessage);
            return response.Result;
        }

        public HttpResponseMessage Get(string url, IDictionary<string, string> headers)
        {
            return Send(BuildRequestMessage(HttpMethod.Get, url, headers));
        }

        private HttpRequestMessage BuildRequestMessage(HttpMethod httpMethod, string url, IDictionary<string, string> headers)
        {
            var message = new HttpRequestMessage(httpMethod, url);

            if (headers != null && headers.Count > 0)
            {
                foreach(var header in headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }

            return message;
        }
    }
}
