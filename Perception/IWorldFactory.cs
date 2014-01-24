using System;
using System.Net;

namespace Perception
{
    public interface IWorldFactory
    {
        PerceptionWorld CreatePerceptionWorld(bool join, IPAddress address);
    }
}

