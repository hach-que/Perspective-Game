using System;
using Protogame;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;

namespace Perception
{
    public class DefaultNetworkAPI : INetworkAPI
    {
        private readonly Dictionary<string, List<Action<string>>> m_MessageEvents;

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
            this.m_MxDispatcher.ClientDisconnectWarning += this.OnClientDisconnectWarning;

            this.m_MessageEvents = new Dictionary<string, List<Action<string>>>();

            this.WasJoin = join;

            if (join)
            {
                this.m_MxDispatcher.Connect(new DualIPEndPoint(address, 9000, 9001));
            }
        }

        public void ClearAllListeners()
        {
            this.m_MessageEvents.Clear();
        }

        public bool HasOtherPlayer
        {
            get
            {
                return this.m_MxDispatcher.Endpoints.Count() >= 1;
            }
        }

        public bool WasJoin
        {
            get;
            private set;
        }

        public void ListenForMessage(string type, Action<string> callback)
        {
            if (!this.m_MessageEvents.ContainsKey(type))
            {
                this.m_MessageEvents[type] = new List<Action<string>>();
            }

            this.m_MessageEvents[type].Add(callback);
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
            // TODO
        }

        public void Update()
        {
            this.ClientDisconnectAccumulator = 0;
            this.m_MxDispatcher.Update();
        }

        public float ClientDisconnectAccumulator
        {
            get;
            set;
        }

        public bool Disconnected
        {
            get;
            set;
        }

        private void OnClientDisconnectWarning(object sender, MxDisconnectEventArgs e)
        {
            this.ClientDisconnectAccumulator = e.DisconnectAccumulator;
        }

        private void OnMessageReceived(object sender, MxMessageEventArgs e)
        {
            var components = Encoding.ASCII.GetString(e.Payload).Split(new[] { '|' }, 2);

            if (this.m_MessageEvents.ContainsKey(components[0]))
            {
                foreach (var callback in this.m_MessageEvents[components[0]])
                {
                    callback(components[1]);
                }
            }
        }

        private void OnClientDisconnected(object sender, MxClientEventArgs e)
        {
            this.Disconnected = true;
        }
    }
}

