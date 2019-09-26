using System.Collections.Generic;

namespace Magis.School.ApiClient.Events.Messages
{
    public class UpdateMessage : IMessage
    {
        public UpdateEvent Event { get; set; }

        public IDictionary<string, object> Context { get; set; }

        public string Target { get; set; }
    }
}
