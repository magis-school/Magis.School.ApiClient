using System.Collections.Generic;

namespace Magis.School.ApiClient.Events.Messages
{
    public class UpdateMessage : IMessage
    {
        public UpdateEvent Event { get; }

        public IDictionary<string, object> Context { get; }

        public string Target { get; }
    }
}
