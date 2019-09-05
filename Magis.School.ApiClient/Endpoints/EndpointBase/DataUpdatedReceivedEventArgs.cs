using System;
using System.Collections.Generic;
using Magis.School.ApiClient.Events;

namespace Magis.School.ApiClient.Endpoints.EndpointBase
{
    public class DataUpdatedReceivedEventArgs : EventArgs
    {
        public UpdateEvent UpdateEvent { get; }

        public IDictionary<string, object> Context { get; }

        public string Target { get; }

        public DataUpdatedReceivedEventArgs(UpdateEvent updateEvent, IDictionary<string, object> context, string target)
        {
            UpdateEvent = updateEvent;
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }
    }
}
