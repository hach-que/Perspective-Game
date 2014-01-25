using System;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

namespace Perception
{
    public class GoalEntity : BaseNetworkEntity
    {
        private ModelAsset m_GoalModel;

        private int m_Rotation;

        public GoalEntity(
            I2DRenderUtilities twodRenderUtilities,
            ICubeRenderer cubeRenderer,
            IAssetManagerProvider assetManagerProvider,
            INetworkAPI networkAPI,
            int id,
            int x,
            int y,
            Dictionary<string, string> attributes)
            : base(twodRenderUtilities,
                cubeRenderer,
                assetManagerProvider,
                networkAPI,
                Convert.ToInt32(attributes["NetworkID"]))
        {
            this.X = x / 16f + 0.5f;
            this.Z = y / 16f + 0.5f;
            this.CanPickup = false;
            this.JoinShouldOwn = Convert.ToBoolean(attributes["JoinOwns"]);

            this.m_GoalModel = assetManagerProvider.GetAssetManager().Get<ModelAsset>("model.Goal");
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            base.Update(gameContext, updateContext);

            var players = gameContext.World.Entities.OfType<PlayerEntity>();

            foreach (var player in players.ToArray())
            {
                var target = new Vector3(this.X, this.Y, this.Z);
                var source = new Vector3(player.X, player.Y, player.Z);

                if (!player.IsOnFloor(gameContext))
                {
                    continue;
                }

                if ((target - source).Length() < 0.25f)
                {
                    ((PerceptionWorld)gameContext.World).InitiateNextLevel();

                    return;
                }
            }
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            renderContext.SetActiveTexture(renderContext.SingleWhitePixel);

            this.m_GoalModel.Draw(
                renderContext,
                Matrix.CreateScale(0.4f) * 
                Matrix.CreateRotationY(MathHelper.ToRadians(this.m_Rotation++)) *
                Matrix.CreateTranslation(this.X, this.Y + 1f, this.Z),
                this.m_GoalModel.AvailableAnimations.First().Name,
                TimeSpan.Zero);
        }
    }
}

