using System;
using Protogame;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Perception
{
	public class MoveUpAction : IEventAction<IGameContext>
	{
        public void Handle(IGameContext context, Event @event)
        {
            var player = context.World.Entities.OfType<PlayerEntity>().FirstOrDefault();

            if (player == null)
            {
                return;
            }

            if (player.Z >= 0)
            {
                player.Z -= 0.1f;
            }
        }
	}
}

