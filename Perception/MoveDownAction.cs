using System;
using Protogame;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Perception
{
	public class MoveDownAction : IEventAction<IGameContext>
	{
        public void Handle(IGameContext context, Event @event)
        {
            var player = context.World.Entities.OfType<PlayerEntity>().FirstOrDefault();

            if (player == null)
            {
                return;
            }

            player.Y += 4;
        }
	}
}

