using System;
using Protogame;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ninject;
using System.Net;

namespace Perception
{
    public class IntermissionWorld : IWorld
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly FontAsset m_MessageFont;

        private readonly INetworkAPI m_NetworkAPI;

        private int m_Ticks;

        private int m_ApproachingLevel;

        private bool m_NoNextLevel;

        public IntermissionWorld(
            IKernel kernel,
            INetworkAPI networkAPI,
            I2DRenderUtilities twodRenderUtilities,
            IAssetManagerProvider assetManagerProvider, 
            int level)
        {
            this.m_2DRenderUtilities = twodRenderUtilities;
            this.m_MessageFont = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Message");
            this.m_NetworkAPI = networkAPI;

            this.Entities = new List<IEntity>();

            this.m_Ticks = 0;
            this.m_ApproachingLevel = level;

            this.m_NetworkAPI.ClearAllListeners();

            this.m_NoNextLevel =
                (assetManagerProvider.GetAssetManager().TryGet<LevelAsset>("level." + level + "a") == null);
        }

        public string GetLevelMessage()
        {
            if (this.m_NoNextLevel)
            {
                return @"Programming: James Rhodes
Artwork: Olek Kalinowski";
            }

            switch (this.m_ApproachingLevel)
            {
                case 1:
                    return "We all see things differently.";
                case 2:
                    return "You see further from the mountain.";
                case 3:
                    return "Nobody is wrong.";
                case 4:
                    return "Everyone is right.";
                case 5:
                    return "Perspective.";
                default:
                    return "???";
            }
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

            var fadeValue = 1f;
            if (this.m_Ticks < 60)
            {
                fadeValue = this.m_Ticks / 60f;
            }
            if (this.m_Ticks >= (60 * 4))
            {
                fadeValue = ((60 * 5) - this.m_Ticks) / 60f;
            }

            renderContext.GraphicsDevice.Clear(Color.Black);

            var color = new Color(0f, 0f, 0f, 1f - fadeValue);

            var i = 0;
            foreach (var line in this.GetLevelMessage().Split('\n'))
            {
                this.m_2DRenderUtilities.RenderText(
                    renderContext,
                    new Vector2(
                        renderContext.GraphicsDevice.Viewport.Width / 2,
                        renderContext.GraphicsDevice.Viewport.Height / 2 - this.GetLevelMessage().Split('\n').Length / 2f + i * 40),
                    line,
                    this.m_MessageFont,
                    horizontalAlignment: HorizontalAlignment.Center,
                    verticalAlignment: VerticalAlignment.Center,
                    textColor: Color.White);
                i++;
            }

            this.m_2DRenderUtilities.RenderRectangle(
                renderContext,
                new Rectangle(
                    0, 0,
                    renderContext.GraphicsDevice.Viewport.Width,
                    renderContext.GraphicsDevice.Viewport.Height),
                color,
                true);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (this.m_Ticks > 60f * 5)
            {
                if (this.m_NoNextLevel)
                {
                    gameContext.Game.Exit();
                    return;
                }
                else
                {
                    gameContext.SwitchWorld<IWorldFactory>(
                        x => x.CreatePerceptionWorld(this.m_ApproachingLevel));
                }
            }

            this.m_NetworkAPI.Update();

            this.m_Ticks++;
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

