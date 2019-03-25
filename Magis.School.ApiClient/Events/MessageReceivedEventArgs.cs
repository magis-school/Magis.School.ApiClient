using System;
using Magis.School.ApiClient.Events.Messages;

namespace Magis.School.ApiClient.Events
{
    internal class MessageReceivedEventArgs : EventArgs
    {
        internal IMessage Message { get; }

        internal MessageReceivedEventArgs(IMessage message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
