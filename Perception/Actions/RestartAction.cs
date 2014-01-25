using System;
using Protogame;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Perception
{
    public class RestartAction : IEventAction<IGameContext>
	{
        public void Handle(IGameContext context, Event @event)
        {
            var world = (PerceptionWorld)context.World;

            world.InitiateResetLevel();
        }
	}
}

