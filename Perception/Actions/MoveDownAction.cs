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
            if (!(context.World is PerceptionWorld))
            {
                return;
            }

            var player = context.World.Entities.OfType<PlayerEntity>().FirstOrDefault(x => x.LocallyOwned);

            if (player == null)
            {
                return;
            }

            if (player.Z < 10 && player.CanMoveTo(context, player.X, player.Z + 0.1f, ignoreCrates: true))
            {
                if (player.CheckAndImpact(context, 0f, 0f, 0.1f))
                {
                    player.Z += 0.1f;
                }
            }
        }
	}
}

