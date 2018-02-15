using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Helpers
{
    public interface IHttpHandler
    {
        HttpResponseMessage Get(string url, IDictionary<string, string> headers);

        HttpResponseMessage Send(HttpRequestMessage requestMessage);
    }
}
