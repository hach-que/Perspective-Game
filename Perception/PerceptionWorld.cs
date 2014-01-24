namespace Perception
{
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;

    using Protogame;

    public class PerceptionWorld : IWorld
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly IAssetManager m_AssetManager;

        private readonly FontAsset m_DefaultFont;

        private readonly IEntityFactory m_EntityFactory;

        private readonly int[,] m_GameBoard;

        public PerceptionWorld(
            I2DRenderUtilities twoDRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IEntityFactory entityFactory)
        {
            this.Entities = new List<IEntity>();

            this.m_2DRenderUtilities = twoDRenderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.m_EntityFactory = entityFactory;

            this.m_GameBoard = new int[10, 10];

            for (var x = 0; x < 10; x++)
            {
                for (var y = 0; y < 10; y++)
                {
                    this.m_GameBoard[x, y] = 0;
                }
            }

            this.Entities.Add(this.m_EntityFactory.CreatePlayerEntity());
        }

        public List<IEntity> Entities { get; private set; }

        public void Dispose()
        {
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                return;
            }

            gameContext.Graphics.GraphicsDevice.Clear(Color.Purple);

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

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }
    }
}
