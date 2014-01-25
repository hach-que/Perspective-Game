using System;
using Protogame;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ninject;
using System.Net;

namespace Perception
{
    public class LobbyWorld : IWorld
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly FontAsset m_DefaultFont;

        private readonly INetworkAPI m_NetworkAPI;

        public LobbyWorld(
            IKernel kernel,
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider, 
            bool join,
            IPAddress address)
        {
            this.m_2DRenderUtilities = twodRenderUtilities;
            this.m_DefaultFont = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");

            this.Entities = new List<IEntity>();

            this.m_NetworkAPI = new DefaultNetworkAPI(join, address);
            kernel.Bind<INetworkAPI>().ToMethod(x => this.m_NetworkAPI);
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                return;
            }

            renderContext.GraphicsDevice.Clear(Color.Black);

            if (!this.m_NetworkAPI.HasOtherPlayer)
            {
                this.m_2DRenderUtilities.RenderText(
                    renderContext,
                    new Vector2(
                        renderContext.GraphicsDevice.Viewport.Width / 2,
                        renderContext.GraphicsDevice.Viewport.Height / 2),
                    "Waiting for another player",
                    this.m_DefaultFont,
                    horizontalAlignment: HorizontalAlignment.Center,
                    verticalAlignment: VerticalAlignment.Center,
                    textColor: Color.Gray);
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (this.m_NetworkAPI.HasOtherPlayer)
            {
                gameContext.SwitchWorld<IWorldFactory>(
                    x => x.CreateIntermissionWorld(1));
            }

            this.m_NetworkAPI.Update();
        }

        public System.Collections.Generic.List<IEntity> Entities
        {
            get;
            private set;
        }

        public void Dispose()
        {
        }
    }
}

