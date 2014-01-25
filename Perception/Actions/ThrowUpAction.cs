using System;
using Protogame;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Perception
{
    public class ThrowUpAction : IEventAction<IGameContext>
	{
        public void Handle(IGameContext context, Event @event)
        {
            var player = context.World.Entities.OfType<PlayerEntity>().FirstOrDefault(x => x.LocallyOwned);

            if (player == null)
            {
                return;
            }

            player.Throw(0f, 0.75f, -0.5f);
        }
	}
}

