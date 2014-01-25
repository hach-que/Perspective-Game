using System;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

namespace Perception
{
    public class DoorEntity : BaseNetworkEntity
    {
        private ModelAsset m_DoorClosedModel;

        private ModelAsset m_DoorOpenModel;

        public DoorEntity(
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
            this.X = x * 16;
            this.Z = y * 16;

            this.m_DoorClosedModel = assetManagerProvider.GetAssetManager().Get<ModelAsset>("model.DoorClosed");
            this.m_DoorOpenModel = assetManagerProvider.GetAssetManager().Get<ModelAsset>("model.DoorOpen");
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            this.m_DoorClosedModel.Draw(
                renderContext,
                Matrix.CreateScale(1f, 1f, 0.2f) *
                Matrix.CreateScale(0.5f) *
                Matrix.CreateTranslation(this.X, this.Y, this.Z),
                Animation.AnimationNullName,
                TimeSpan.Zero);
        }
    }
}

