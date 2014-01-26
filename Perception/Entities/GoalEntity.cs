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

        private TextureAsset m_GoalTexture;

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
            this.X = x / 16f + 0.4f;
            this.Z = y / 16f + 0.4f;
            this.CanPickup = false;
            this.JoinShouldOwn = Convert.ToBoolean(attributes["JoinOwns"]);

            this.Width = 0.2f;
            this.Depth = 0.2f;

            this.m_GoalTexture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("texture.Goal");
            this.m_GoalModel = assetManagerProvider.GetAssetManager().Get<ModelAsset>("model.Goal");
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            base.Update(gameContext, updateContext);

            var players = gameContext.World.Entities.OfType<PlayerEntity>();

            foreach (var player in players.ToArray())
            {
                var target = new Vector3(this.X + 0.1f, this.Y, this.Z + 0.1f);
                var source = new Vector3(player.X + player.Width / 2, player.Y, player.Z + player.Depth / 2);

                if (!player.IsOnFloor(gameContext))
                {
                    continue;
                }

                if ((target - source).Length() < 0.4f)
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

            renderContext.SetActiveTexture(this.m_GoalTexture.Texture);

            this.m_GoalModel.Draw(
                renderContext,
                Matrix.CreateScale(0.4f) * 
                Matrix.CreateRotationY(MathHelper.ToRadians(this.m_Rotation++)) *
                Matrix.CreateTranslation(this.X + 0.1f, this.Y + 1f, this.Z + 0.1f),
                this.m_GoalModel.AvailableAnimations.First().Name,
                TimeSpan.Zero);
        }
    }
}

