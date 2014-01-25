using System;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

namespace Perception
{
    public class KeyEntity : BaseNetworkEntity
    {
        private ModelAsset m_KeyModel;

        private int midx;

        public KeyEntity(
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
            this.JoinShouldOwn = Convert.ToBoolean(attributes["JoinOwns"]);
            this.CanPickup = true;

            this.m_KeyModel = assetManagerProvider.GetAssetManager().Get<ModelAsset>("model.Key");
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            midx++;

            renderContext.SetActiveTexture(renderContext.SingleWhitePixel);

            this.m_KeyModel.Draw(
                renderContext,
                Matrix.CreateScale(1f, 1f, 0.2f) *
                Matrix.CreateScale(0.5f) *
                Matrix.CreateRotationY(MathHelper.ToRadians(midx)) *
                Matrix.CreateTranslation(this.X, this.Y + 1, this.Z),
                Animation.AnimationNullName,
                TimeSpan.Zero);
        }
    }
}

