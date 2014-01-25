using System;
using Protogame;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Perception
{
    public class PickupDropAction : IEventAction<IGameContext>
	{
        public void Handle(IGameContext context, Event @event)
        {
            var player = context.World.Entities.OfType<PlayerEntity>().FirstOrDefault(x => x.LocallyOwned);

            if (player == null)
            {
                return;
            }

            if (player.HoldingObject)
            {
                player.Throw();
                return;
            }

            var min = 10000f;
            BaseNetworkEntity entityChosen = null;
            var target = new Vector3(player.X, player.Y, player.Z);

            foreach (var entity in context.World.Entities.OfType<BaseNetworkEntity>())
            {
                var source = new Vector3(entity.X, entity.Y, entity.Z);

                if (!entity.CanPickup)
                {
                    continue;
                }

                if ((target - source).LengthSquared() < min)
                {
                    entityChosen = entity;
                    min = (target - source).LengthSquared();
                }
            }

            if (entityChosen == null)
            {
                return;
            }

            if (Math.Sqrt(min) < 1)
            {
                player.Pickup(entityChosen);
            }
        }
	}
}

