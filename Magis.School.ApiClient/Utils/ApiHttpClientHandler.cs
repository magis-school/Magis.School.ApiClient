using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Magis.School.ApiClient.Utils
{
    public class ApiHttpClientHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Set CORS-Headers
            request.Headers.Add("X-Requested-With", "C#-API-Client");

            return base.SendAsync(request, cancellationToken);
        }
    }
}
