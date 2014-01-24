using System;
using Protogame;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Perception
{
    public class JumpAction : IEventAction<IGameContext>
	{
        public void Handle(IGameContext context, Event @event)
        {
            var player = context.World.Entities.OfType<PlayerEntity>().FirstOrDefault(x => x.LocallyOwned);

            if (player == null)
            {
                return;
            }

            // TODO: Check if player can jump.
            player.YSpeed = 0.85f;
        }
	}
}
