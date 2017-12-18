using System;
using System.Runtime.Serialization;
using Sharpcaster.Core.Exceptions;

namespace Sharpcaster.Core.Messages.Media
{
    /// <summary>
    /// Load failed message
    /// </summary>
    [DataContract]
    [ReceptionMessage]
    class LoadFailedMessage : MessageWithId
    {
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            throw new MediaLoadFailedException();
        }
    }
}
