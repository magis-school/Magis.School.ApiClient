using System;
using Magis.School.ApiClient.Endpoints;
using Magis.School.ApiClient.Serialization;

namespace Magis.School.ApiClient
{
    public class ApiClient
    {
        public string ServerBackendUrl { get; }

        private readonly JsonSettings _jsonSerializerSettings;

        public ApiClient(string serverBackendUrl)
        {
            ServerBackendUrl = serverBackendUrl ?? throw new ArgumentNullException(nameof(serverBackendUrl));
            _jsonSerializerSettings = new JsonSettings();
        }

        public ComputersEndpoint Computers(string computerToken)
        {
            return new ComputersEndpoint(ServerBackendUrl, computerToken, _jsonSerializerSettings);
        }
    }
}
