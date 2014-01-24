using Microsoft.Xna.Framework.Graphics;
using System;
using System.Net;
using Ninject;
using System.Linq;

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

        private readonly int[,] m_GameBoardTX;

        private readonly int[,] m_GameBoardTY;

        private readonly PlayerEntity m_MyPlayer;

        private readonly PlayerEntity m_OtherPlayer;

        private readonly ICubeRenderer m_CubeRenderer;

        private readonly TextureAsset m_CubeTexture;

        private readonly INetworkAPI m_NetworkAPI;

        private readonly char m_LevelSuffix;

        private readonly ILevelManager m_LevelManager;

        public PerceptionWorld(
            IKernel kernel,
            I2DRenderUtilities twoDRenderUtilities,
            I3DRenderUtilities threeDRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IEntityFactory entityFactory,
            ICubeRenderer cubeRenderer,
            ILevelManager levelManager,
            bool join,
            IPAddress address)
        {
            this.Entities = new List<IEntity>();

            this.m_NetworkAPI = new DefaultNetworkAPI(join, address);
            kernel.Bind<INetworkAPI>().ToMethod(x => this.m_NetworkAPI);

            this.m_2DRenderUtilities = twoDRenderUtilities;
            this.m_3DRenderUtilities = threeDRenderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.m_EntityFactory = entityFactory;
            this.m_CubeRenderer = cubeRenderer;
            this.m_CubeTexture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("texture.Terrain");
            this.m_LevelManager = levelManager;

            this.m_GameBoard = new int[10, 10];
            this.m_GameBoardTX = new int[10, 10];
            this.m_GameBoardTY = new int[10, 10];

            this.m_LevelSuffix = join ? 'b' : 'a';

            this.LoadLevel(1);

            this.m_MyPlayer = this.m_EntityFactory.CreatePlayerEntity(join, true);
            this.m_OtherPlayer = this.m_EntityFactory.CreatePlayerEntity(!join, false);

            this.Entities.Add(this.m_OtherPlayer);
            this.Entities.Add(this.m_MyPlayer);
        }

        public int[,] GameBoard { get { return this.m_GameBoard; } }

        public List<IEntity> Entities { get; private set; }

        public void Dispose()
        {
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void LoadLevel(int levelNum)
        {
            this.m_LevelManager.Load(this, "level." + levelNum + this.m_LevelSuffix);

            foreach (var levelTile in this.Entities.OfType<LevelTileEntity>())
            {
                this.m_GameBoardTX[(int)levelTile.X, (int)levelTile.Y] = levelTile.TX;
                this.m_GameBoardTY[(int)levelTile.X, (int)levelTile.Y] = levelTile.TY;

                switch (levelTile.TY)
                {
                    case 0:
                        switch (levelTile.TX)
                        {
                            case 0:
                                this.m_GameBoard[(int)levelTile.X, (int)levelTile.Y] = 2;
                                break;
                            case 1:
                                this.m_GameBoard[(int)levelTile.X, (int)levelTile.Y] = 1;
                                break;
                            case 2:
                                this.m_GameBoard[(int)levelTile.X, (int)levelTile.Y] = 3;
                                break;
                            default:
                                break;
                        }
                    default:
                        break;
                }
            }

            this.Entities.RemoveAll(x => x is LevelTileEntity);
        }

        private int midx = 0;

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                // set up 3d camera for rendering

                renderContext.View = 
                    Matrix.CreateLookAt(
                        new Vector3(this.m_MyPlayer.X * 1.05f, 15, this.m_MyPlayer.Z * 1.05f + 5),
                        new Vector3(this.m_MyPlayer.X, this.m_MyPlayer.Y, this.m_MyPlayer.Z),
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
                            this.m_CubeTexture,
                            new Vector2(this.m_GameBoardTX[x, y] / 8f, this.m_GameBoardTY[x, y] / 4f),
                            new Vector2(this.m_GameBoardTX[x, y] / 8f, this.m_GameBoardTY[x, y] / 4f));
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

                if (this.m_NetworkAPI.ClientDisconnectAccumulator > 0)
                {
                    this.m_2DRenderUtilities.RenderText(
                        renderContext,
                        new Vector2(10, 50),
                        "Client disconnect accumulator: " + this.m_NetworkAPI.ClientDisconnectAccumulator,
                        this.m_DefaultFont,
                        textColor: Color.Red);
                }
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_NetworkAPI.Update();

            if (this.m_NetworkAPI.Disconnected)
            {
                gameContext.Game.Exit();
            }
        }
    }
}
