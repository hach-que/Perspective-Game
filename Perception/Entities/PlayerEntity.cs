using System;
using Protogame;
using Microsoft.Xna.Framework;

namespace Perception
{
    public class PlayerEntity : IEntity
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        public PlayerEntity(I2DRenderUtilities twodRenderUtilities)
        {
            this.m_2DRenderUtilities = twodRenderUtilities;
        }

        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        public float Z
        {
            get;
            set;
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {

        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                return;
            }

            this.m_2DRenderUtilities.RenderRectangle(
                renderContext,
                new Rectangle((int)this.X, (int)this.Y, 20, 20),
                Color.Red,
                true);
        }
    }
}

