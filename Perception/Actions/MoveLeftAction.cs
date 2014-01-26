using System;
using Protogame;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Perception
{
	public class MoveLeftAction : IEventAction<IGameContext>
	{
        public void Handle(IGameContext context, Event @event)
        {
            if (!(context.World is PerceptionWorld))
            {
                return;
            }

            var player = context.World.Entities.OfType<PlayerEntity>().FirstOrDefault(x => x.LocallyOwned);

            if (player == null)
            {
                return;
            }

            if (player.X >= 0 && player.CanMoveTo(context, player.X - 0.1f, player.Z, ignoreCrates: true))
            {
                if (player.CheckAndImpact(context, -0.1f, 0f, 0f))
                {
                    player.X -= 0.1f;
                }
            }
        }
	}
}

