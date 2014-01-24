using System;

namespace Perception
{
    public interface INetworkAPI
    {
        void SendMessage(string type, string data);

        void StopListeningForMessage(string type);

        void ListenForMessage(string type, Action<string> callback);

        void Update();
    }
}

