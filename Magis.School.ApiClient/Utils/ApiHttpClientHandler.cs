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
        private readonly string _userName;
        private readonly string _password;

        public ApiHttpClientHandler(string userName, string password)
        {
            _userName = userName ?? throw new ArgumentNullException(nameof(userName));
            _password = password ?? throw new ArgumentNullException(nameof(password));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // ggf. Authentifizierung setzen
            if (request.Headers.Authorization != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userName}:{_password}")));

            // CORS-Header setzen
            request.Headers.Add("X-Requested-With", "C#-API-Client");

            return base.SendAsync(request, cancellationToken);
        }
    }
}
