using System;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Perception
{
    public class PlayerEntity : IEntity
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly ICubeRenderer m_CubeRenderer;

        private readonly TextureAsset m_PlayerTexture;

        public PlayerEntity(
            I2DRenderUtilities twodRenderUtilities,
            ICubeRenderer cubeRenderer,
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_2DRenderUtilities = twodRenderUtilities;
            this.m_CubeRenderer = cubeRenderer;
            this.m_PlayerTexture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("texture.Terrain");

            this.X = 5.5f;
            this.Y = 1f;
            this.Z = 5.5f;
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
                this.m_CubeRenderer.RenderCube(
                    renderContext,
                    Matrix.CreateScale(0.5f) *
                    Matrix.CreateTranslation(new Vector3(this.X - 0.25f, this.Y, this.Z - 0.25f)),
                    this.m_PlayerTexture);
            }
            else
            {
            }
        }
    }
}

