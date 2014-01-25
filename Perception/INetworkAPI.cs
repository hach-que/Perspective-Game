using System;

namespace Perception
{
    public interface INetworkAPI
    {
        float ClientDisconnectAccumulator { get; }

        bool Disconnected { get; }

        bool HasOtherPlayer { get; }

        bool WasJoin { get; }

        void ClearAllListeners();

        void SendMessage(string type, string data);

        void StopListeningForMessage(string type);

        void ListenForMessage(string type, Action<string> callback);

        void Update();
    }
}

