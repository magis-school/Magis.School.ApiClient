using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Magis.School.ApiClient.Utils
{
    public class BasicAuthApiHttpClientHandler : ApiHttpClientHandler
    {
        private readonly string _userName;
        private readonly string _password;

        public BasicAuthApiHttpClientHandler(string userName, string password)
        {
            _userName = userName ?? throw new ArgumentNullException(nameof(userName));
            _password = password ?? throw new ArgumentNullException(nameof(password));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Set authentication if necessary
            if (request.Headers.Authorization != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userName}:{_password}")));

            return base.SendAsync(request, cancellationToken);
        }
    }
}
