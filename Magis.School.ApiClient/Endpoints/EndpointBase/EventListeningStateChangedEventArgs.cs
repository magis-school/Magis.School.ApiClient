using System;

namespace Magis.School.ApiClient.Endpoints.EndpointBase
{
    public class EventListeningStateChangedEventArgs : EventArgs
    {
        public EventListeningState State { get; }

        public EventListeningStateChangedEventArgs(EventListeningState state)
        {
            State = state;
        }
    }
}
