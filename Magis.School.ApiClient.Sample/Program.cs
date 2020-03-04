using System;
using System.Threading.Tasks;
using Magis.School.ApiClient.DataObjects;
using Magis.School.ApiClient.Endpoints;
using Magis.School.ApiClient.Endpoints.EndpointBase;

namespace Magis.School.ApiClient.Sample
{
    // This sample application demonstrates how a vnc container could use the api-client
    // to retrieve information and update-events about its current environment.
    public static class Program
    {
        private const string ServerBackendUrl = "http://10.200.1.1";

        public static async Task Main(string[] args)
        {
            // Just some code for graceful application exiting
            var exitingTcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, eventArgs) => exitingTcs.TrySetResult(null);

            // Create API client object
            var apiClient = new MagisSchoolApiClient(ServerBackendUrl);

            // Configure endpoint and authenticate as vnc-container
            VncContainersEndpoint vncContainersEndpoint = apiClient.VncContainers("vnc-container-test-admin-officialappsbeta-libreoffice-7b823492-b0b6-4e60-a050-f78b8e2aa010",
                "f7b6c6b1f31599b3972d3b97ed0a1667");

            // Log "low-level" SSE events for clarity/debugging
            vncContainersEndpoint.DataUpdatedReceived += (sender, eventArgs) => Console.WriteLine($"SSE Update-Event: {eventArgs.UpdateEvent} {eventArgs.Target}");
            vncContainersEndpoint.EventListeningErrorOccured += (sender, eventArgs) => Console.WriteLine($"SSE Listening Error: {eventArgs.Exception}");
            vncContainersEndpoint.EventListeningStateChanged += (sender, eventArgs) => Console.WriteLine($"SSE Listening State Changed: {eventArgs.State}");

            Console.WriteLine("Querying data and watching for updates...");

            // Query container environment as self-managing data object (gets updated automatically)
            VncContainerEnvironmentDataObject vncContainerEnvironment =  vncContainersEndpoint.GetVncContainerEnvironment();
            await vncContainerEnvironment.EnsureLoadedAsync().ConfigureAwait(false);

            // You can work with this data object now...

            // Wait for application exit
            await exitingTcs.Task.ConfigureAwait(false);

            Console.WriteLine("Exiting...");

            // Cleanup
            vncContainerEnvironment.Dispose();
            if (vncContainersEndpoint.CurrentEventListeningState != EventListeningState.Stopped)
                await vncContainersEndpoint.StopListeningForEventsAsync().ConfigureAwait(false);
            vncContainersEndpoint.Dispose();
        }
    }
}
