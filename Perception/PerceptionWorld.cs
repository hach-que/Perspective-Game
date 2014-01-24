using Microsoft.Xna.Framework.Graphics;
using System;

namespace Perception
{
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;

    using Protogame;

    public class PerceptionWorld : IWorld
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly I3DRenderUtilities m_3DRenderUtilities;

        private readonly IAssetManager m_AssetManager;

        private readonly FontAsset m_DefaultFont;

        private readonly IEntityFactory m_EntityFactory;

        private readonly int[,] m_GameBoard;

        private readonly PlayerEntity m_player;

        private readonly ICubeRenderer m_CubeRenderer;

        private readonly TextureAsset m_CubeTexture;

        public PerceptionWorld(
            I2DRenderUtilities twoDRenderUtilities,
            I3DRenderUtilities threeDRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IEntityFactory entityFactory,
            ICubeRenderer cubeRenderer)
        {
            this.Entities = new List<IEntity>();

            this.m_2DRenderUtilities = twoDRenderUtilities;
            this.m_3DRenderUtilities = threeDRenderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.m_EntityFactory = entityFactory;
            this.m_CubeRenderer = cubeRenderer;
            this.m_CubeTexture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("texture.Terrain");

            this.m_GameBoard = new int[10, 10];

            var testboard = 
@"1111111111
1111122222
1111122222
1111122222
2221111111
2221111111
2221111111
1111111111
1111111111
1111111111";

            var lines = testboard.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            var random = new Random();
            for (var x = 0; x < 10; x++)
            {
                for (var y = 0; y < 10; y++)
                {
                    this.m_GameBoard[x, y] = int.Parse(lines[y][x] + "");
                }
            }

            this.m_player = this.m_EntityFactory.CreatePlayerEntity();

            this.Entities.Add(m_player);
        }

        public List<IEntity> Entities { get; private set; }

        public void Dispose()
        {
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        private int midx = 0;

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                // set up 3d camera for rendering

                renderContext.View = 
                    Matrix.CreateLookAt(
                        new Vector3(this.m_player.X * 1.05f, 15, this.m_player.Z * 1.05f + 2),
                        new Vector3(this.m_player.X, 0f, this.m_player.Z),
                        new Vector3(0, 0, -1));
                renderContext.Projection =
                    Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.PiOver4,
                        renderContext.GraphicsDevice.Viewport.Width / (float)renderContext.GraphicsDevice.Viewport.Height,
                        1.0f,
                        5000.0f);

                renderContext.GraphicsDevice.Clear(Color.Black);

                var basicEffect = (BasicEffect)renderContext.Effect;

                basicEffect.EnableDefaultLighting();

                basicEffect.PreferPerPixelLighting = true;

                // render game grid

                for (var x = 0; x < 10; x++)
                {
                    for (var y = 0; y < 10; y++)
                    {
                        this.m_CubeRenderer.RenderCube(
                            renderContext,
                            Matrix.CreateScale(1, this.m_GameBoard[x, y], 1) *
                            Matrix.CreateTranslation(new Vector3(x, 0, y)),
                            this.m_CubeTexture);
                    }
                }
            }
            else
            {
                this.m_2DRenderUtilities.RenderText(
                    renderContext,
                    new Vector2(10, 10),
                    "Hello Perception!",
                    this.m_DefaultFont);

                this.m_2DRenderUtilities.RenderText(
                    renderContext,
                    new Vector2(10, 30),
                    "Running at " + gameContext.FPS + " FPS; " + gameContext.FrameCount + " frames counted so far",
                    this.m_DefaultFont);
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }
    }
}
