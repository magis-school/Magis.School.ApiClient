using System;
using Magis.School.ApiClient.Endpoints;
using Magis.School.ApiClient.Serialization;
using Refit;

namespace Magis.School.ApiClient
{
    public class MagisSchoolApiClient
    {
        public string ServerBackendUrl { get; }

        private readonly RefitSettings _refitSettings;

        public MagisSchoolApiClient(string serverBackendUrl)
        {
            ServerBackendUrl = serverBackendUrl ?? throw new ArgumentNullException(nameof(serverBackendUrl));

            _refitSettings = new RefitSettings {ContentSerializer = new JsonContentSerializer(new JsonSettings()),};
        }

        public WebEndpoint Web() => new WebEndpoint(_refitSettings, ServerBackendUrl);

        public ComputersEndpoint Computers(string computerToken) => new ComputersEndpoint(_refitSettings, ServerBackendUrl, computerToken);

        public VncContainersEndpoint VncContainers(string vncContainerName, string apiToken) =>
            new VncContainersEndpoint(_refitSettings, ServerBackendUrl, vncContainerName, apiToken);
    }
}
