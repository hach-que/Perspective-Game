using System;
using Protogame;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Perception
{
    public class SkipLevelAction : IEventAction<IGameContext>
	{
        public void Handle(IGameContext context, Event @event)
        {
            if (!(context.World is PerceptionWorld))
            {
                return;
            }

            var world = (PerceptionWorld)context.World;

            world.InitiateNextLevel();
        }
	}
}

