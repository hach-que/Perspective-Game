using System;
using System.Net;

namespace Perception
{
    public interface IWorldFactory
    {
        LobbyWorld CreateLobbyWorld(bool join, IPAddress address);
        IntermissionWorld CreateIntermissionWorld(int level);
        PerceptionWorld CreatePerceptionWorld(int level);
    }
}

