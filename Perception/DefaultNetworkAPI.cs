using System;
using Protogame;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Perception
{
    public class DefaultNetworkAPI : INetworkAPI
    {
        private readonly Dictionary<string, Action<string>> m_MessageEvents;

        private readonly MxDispatcher m_MxDispatcher;

        public DefaultNetworkAPI(bool join, IPAddress address)
        {
            if (!join)
            {
                this.m_MxDispatcher = new MxDispatcher(9000, 9001);
            }
            else
            {
                this.m_MxDispatcher = new MxDispatcher(9002, 9003);
            }
            this.m_MxDispatcher.MessageReceived += this.OnMessageReceived;
            this.m_MxDispatcher.ClientDisconnected += this.OnClientDisconnected;

            this.m_MessageEvents = new Dictionary<string, Action<string>>();

            if (join)
            {
                this.m_MxDispatcher.Connect(new DualIPEndPoint(address, 9000, 9001));
            }
        }

        public void ListenForMessage(string type, Action<string> callback)
        {
            if (this.m_MessageEvents.ContainsKey(type))
            {
                throw new InvalidOperationException("callback already registered");
            }

            this.m_MessageEvents[type] = callback;
        }

        public void SendMessage(string type, string data)
        {
            foreach (var endpoint in this.m_MxDispatcher.Endpoints)
            {
                this.m_MxDispatcher.Send(endpoint, Encoding.ASCII.GetBytes(type + "|" + data), false);
            }
        }

        public void StopListeningForMessage(string type)
        {
            if (!this.m_MessageEvents.ContainsKey(type))
            {
                throw new InvalidOperationException("callback not registered");
            }

            this.m_MessageEvents.Remove(type);
        }

        public void Update()
        {
            this.m_MxDispatcher.Update();
        }

        private void OnMessageReceived(object sender, MxMessageEventArgs e)
        {
            var components = Encoding.ASCII.GetString(e.Payload).Split(new[] { '|' }, 2);

            if (this.m_MessageEvents.ContainsKey(components[0]))
            {
                this.m_MessageEvents[components[0]](components[1]);
            }
        }

        private void OnClientDisconnected(object sender, MxClientEventArgs e)
        {
            Console.WriteLine("client disconnected! D:");
        }
    }
}

