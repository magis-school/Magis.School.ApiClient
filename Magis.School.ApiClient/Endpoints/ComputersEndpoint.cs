using System;
using System.Net.Http;
using System.Threading.Tasks;
using Magis.School.ApiClient.Endpoints.Computers;
using Magis.School.ApiClient.Events.Messages;
using Magis.School.ApiClient.Utils;
using Newtonsoft.Json;
using Refit;

namespace Magis.School.ApiClient.Endpoints
{
    public class ComputersEndpoint : EndpointWithEventsBase
    {
        public event EventHandler<EventArgs> ComputerSessionStatusChanged;

        public IComputerStatus ComputerStatus { get; }
        internal IComputerEvents ComputerEvents { get; }

        private readonly string _serverBackendUrl;
        private readonly string _computerToken;

        internal ComputersEndpoint(string serverBackendUrl, string computerToken, JsonSerializerSettings jsonSerializerSettings)
        {
            _serverBackendUrl = serverBackendUrl ?? throw new ArgumentNullException(nameof(serverBackendUrl));
            _computerToken = computerToken ?? throw new ArgumentNullException(nameof(computerToken));

            var refitSettings = new RefitSettings {
                JsonSerializerSettings = jsonSerializerSettings,
            };
            var httpClient = new HttpClient(new ApiHttpClientHandler("computer", _computerToken)) {
                BaseAddress = new Uri(serverBackendUrl)
            };

            ComputerStatus = RestService.For<IComputerStatus>(httpClient, refitSettings);
            ComputerEvents = RestService.For<IComputerEvents>(httpClient, refitSettings);
        }

        public Task StartListeningForEventsAsync()
        {
            return StartListeningForEventsInternalAsync(queryEventStreamDelegate: () => ComputerEvents.GetEventStreamAsync());
        }

        public Task StopListeningForEventsAsync() => StopListeningForEventsInternalAsync();

        protected internal override Task HandleEventMessageAsync(IMessage message)
        {
            switch (message)
            {
                case ComputerSessionStatusChangedMessage computerSessionStatusChangedMessage:
                    ComputerSessionStatusChanged?.Invoke(this, EventArgs.Empty);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
