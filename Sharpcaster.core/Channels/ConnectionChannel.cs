﻿using Sharpcaster.Core.Interfaces;
using Sharpcaster.Core.Messages.Connection;
using Sharpcaster.Core.Messages.Receiver;
using Sharpcaster.Core.Models.ChromecastStatus;
using System.Threading.Tasks;

namespace Sharpcaster.Core.Channels
{
    /// <summary>
    /// Connection channel, Responsible for opening connection to Chromecast and receiving Closed message
    /// </summary>
    public class ConnectionChannel : ChromecastChannel, IConnectionChannel
    {
        /// <summary>
        /// Initializes a new instance of ConnectionChannel class
        /// </summary>
        public ConnectionChannel() : base("tp.connection")
        {
        }

        /// <summary>
        /// Connects to chromecast
        /// </summary>
        public async Task ConnectAsync()
        {
            await SendAsync(new ConnectMessage());
        }

        /// <summary>
        /// Connects to running chromecast application 
        /// </summary>
        public async Task ConnectAsync(string transportId)
        {
            await SendAsync(new ConnectMessage(), transportId);
        }

        /// <summary>
        /// Called when a message for this channel is received
        /// </summary>
        /// <param name="message">message to process</param>
        public async override Task OnMessageReceivedAsync(IMessage message)
        {
            if (message is CloseMessage)
            {
                await Client.ReconnectAsync();
            }
            await base.OnMessageReceivedAsync(message);
        }
    }
}
