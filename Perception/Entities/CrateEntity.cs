using System;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

namespace Perception
{
    public class CrateEntity : BaseNetworkEntity
    {
        private ModelAsset m_CrateModel;

        private TextureAsset m_BlueCrateTexture;

        private TextureAsset m_RedCrateTexture;

        public CrateEntity(
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
            this.X = x / 16f;
            this.Z = y / 16f;
            this.JoinShouldOwn = Convert.ToBoolean(attributes["JoinOwns"]);
            this.CanPush = true;

            this.Width = 0.8f;
            this.Depth = 0.8f;

            this.m_BlueCrateTexture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("texture.BlueCrate");
            this.m_RedCrateTexture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("texture.RedCrate");
            this.m_CrateModel = assetManagerProvider.GetAssetManager().Get<ModelAsset>("model.Crate");
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            base.Update(gameContext, updateContext);
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            if (!this.m_NetworkAPI.WasJoin)
            {
                if (this.LocallyOwned)
                {
                    renderContext.SetActiveTexture(this.m_BlueCrateTexture.Texture);
                }
                else
                {
                    renderContext.SetActiveTexture(this.m_RedCrateTexture.Texture);
                }
            }
            else
            {
                if (this.LocallyOwned)
                {
                    renderContext.SetActiveTexture(this.m_RedCrateTexture.Texture);
                }
                else
                {
                    renderContext.SetActiveTexture(this.m_BlueCrateTexture.Texture);
                }
            }

            this.m_CrateModel.Draw(
                renderContext,
                Matrix.CreateScale(0.8f) * 
                Matrix.CreateTranslation(this.X + 0.5f, this.Y + 0.5f, this.Z + 0.4f),
                this.m_CrateModel.AvailableAnimations.First().Name,
                TimeSpan.Zero);
        }
    }
}

