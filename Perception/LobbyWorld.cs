using System;
using Protogame;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Perception
{
    public class LobbyWorld : IWorld
    {
        private readonly IWorldFactory m_WorldFactory;

        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly FontAsset m_DefaultFont;

        public LobbyWorld(
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider, 
            IWorldFactory worldFactory)
        {
            this.m_WorldFactory = worldFactory;
            this.m_2DRenderUtilities = twodRenderUtilities;
            this.m_DefaultFont = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");

            this.Entities = new List<IEntity>();
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            this.m_2DRenderUtilities.RenderText(
                renderContext,
                new Vector2(
                    renderContext.GraphicsDevice.Viewport.Width / 2,
                    renderContext.GraphicsDevice.Viewport.Height / 2),
                "Waiting for another player",
                this.m_DefaultFont,
                horizontalAlignment: HorizontalAlignment.Center,
                verticalAlignment: VerticalAlignment.Center);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
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

