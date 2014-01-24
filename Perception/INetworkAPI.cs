using System;

namespace Perception
{
    public interface INetworkAPI
    {
        float ClientDisconnectAccumulator { get; }

        bool Disconnected { get; }

        void SendMessage(string type, string data);

        void StopListeningForMessage(string type);

        void ListenForMessage(string type, Action<string> callback);

        void Update();
    }
}

