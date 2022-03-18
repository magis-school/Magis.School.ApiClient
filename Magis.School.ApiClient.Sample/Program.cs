using System;
using System.Linq;
using System.Threading.Tasks;
using Magis.School.ApiClient.DataObjects;
using Magis.School.ApiClient.Endpoints;
using Magis.School.ApiClient.Endpoints.EndpointBase;
using Magis.School.ApiClient.Models;

namespace Magis.School.ApiClient.Sample
{
    // This sample application demonstrates how to authenticate against the school server API and query some data.
    public static class Program
    {
        private const string ServerBackendUrl = "https://backend.beta-schule.de";

        public static async Task Main(string[] args)
        {
            // Just some code for graceful application exiting
            var exitingTcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, eventArgs) => exitingTcs.TrySetResult(null);

            // Create API client object
            var apiClient = new MagisSchoolApiClient(ServerBackendUrl);

            // Configure endpoint and authenticate
            WebEndpoint webEndpoint = apiClient.Web();
            User user = await webEndpoint.Auth.LoginAsync(new UserCredentialsInput {UserNameOrMail = "test.benutzer", Password = "Test123"}).ConfigureAwait(false);

            // Log "low-level" SSE events for clarity/debugging
            webEndpoint.DataUpdatedReceived += (sender, eventArgs) => Console.WriteLine($"SSE Update-Event: {eventArgs.UpdateEvent} {eventArgs.Target}");
            webEndpoint.EventListeningErrorOccured += (sender, eventArgs) => Console.WriteLine($"SSE Listening Error: {eventArgs.Exception}");
            webEndpoint.EventListeningStateChanged += (sender, eventArgs) => Console.WriteLine($"SSE Listening State Changed: {eventArgs.State}");

            // Query apps and watch for updates
            AppsDataCollection apps = webEndpoint.GetApps();
            await apps.EnsureLoadedAsync().ConfigureAwait(false);

            // Query courses and watch for updates
            CoursesDataCollection courses = webEndpoint.GetCourses();
            await courses.EnsureLoadedAsync().ConfigureAwait(false);

            // Query favorites and watch for updates
            FavoritesDataCollection favorites = webEndpoint.GetFavorites();
            await favorites.EnsureLoadedAsync().ConfigureAwait(false);

            // Query lessons and watch for updates
            LessonsDataCollection lessons = webEndpoint.GetLessons();
            await lessons.EnsureLoadedAsync().ConfigureAwait(false);

            // Query vnc containers and watch for updates
            VncContainersDataCollection vncContainers = webEndpoint.GetVncContainers();
            await vncContainers.EnsureLoadedAsync().ConfigureAwait(false);

            await Task.Delay(1500000);

            // Wait for application exit
            await exitingTcs.Task.ConfigureAwait(false);
            Console.WriteLine("Exiting...");

            // Cleanup
            apps.Dispose();
            if (webEndpoint.CurrentEventListeningState != EventListeningState.Stopped)
                await webEndpoint.StopListeningForEventsAsync().ConfigureAwait(false);
            webEndpoint.Dispose();
        }
    }
}
