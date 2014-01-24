using System;
using Microsoft.Xna.Framework;

namespace Perception
{
    public interface IEntityFactory
    {
        PlayerEntity CreatePlayerEntity(bool isRedColor, bool locallyOwned);
    }
}

