using System;

namespace Magis.School.ApiClient.Events
{
    public class ListeningStartedEventArgs : EventArgs
    {
        public string EventStreamId { get; }

        public ListeningStartedEventArgs(string eventStreamId)
        {
            EventStreamId = eventStreamId ?? throw new ArgumentNullException(nameof(eventStreamId));
        }
    }
}
