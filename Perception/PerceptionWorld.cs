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

        private readonly string[,] m_GameBoardMeta;

        private readonly int[,] m_GameBoardTX;

        private readonly int[,] m_GameBoardTY;

        private PlayerEntity m_MyPlayer;

        private PlayerEntity m_OtherPlayer;

        private readonly ICubeRenderer m_CubeRenderer;

        private readonly TextureAsset m_CubeTexture;

        private readonly INetworkAPI m_NetworkAPI;

        private readonly char m_LevelSuffix;

        private readonly ILevelManager m_LevelManager;

        private bool m_MoveToNextLevel;

        private bool m_MoveToSameLevel;

        private int m_CurrentLevel;

        public PerceptionWorld(
            IKernel kernel,
            I2DRenderUtilities twoDRenderUtilities,
            I3DRenderUtilities threeDRenderUtilities,
            IAssetManagerProvider assetManagerProvider,
            IEntityFactory entityFactory,
            ICubeRenderer cubeRenderer,
            ILevelManager levelManager,
            INetworkAPI networkAPI,
            int level)
        {
            this.Entities = new List<IEntity>();

            this.m_NetworkAPI = networkAPI;
            this.m_2DRenderUtilities = twoDRenderUtilities;
            this.m_3DRenderUtilities = threeDRenderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.m_EntityFactory = entityFactory;
            this.m_CubeRenderer = cubeRenderer;
            this.m_CubeTexture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("texture.Terrain");
            this.m_LevelManager = levelManager;

            this.m_GameBoard = new int[10, 10];
            this.m_GameBoardMeta = new string[10, 10];
            this.m_GameBoardTX = new int[10, 10];
            this.m_GameBoardTY = new int[10, 10];

            this.m_LevelSuffix = this.m_NetworkAPI.WasJoin ? 'b' : 'a';
            this.m_CurrentLevel = level;

            this.LoadLevel(level);

            this.m_NetworkAPI.ListenForMessage(
                "next level",
                a =>
                {
                    this.HandleNextLevel();
                });

            this.m_NetworkAPI.ListenForMessage(
                "reset level",
                a =>
                {
                    this.HandleResetLevel();
                });

            this.m_NetworkAPI.ListenForMessage(
                "door unlock",
                a =>
                {
                    var values = a.Split('|').Select(x => int.Parse(x)).ToArray();

                    var doorID = values[0];
                    var keyID = values[1];

                    this.Entities.RemoveAll(x => x is KeyEntity && ((KeyEntity)x).ID == keyID);

                    foreach (var door in this.Entities.OfType<DoorEntity>().Where(x=>x.ID == doorID))
                    {
                        door.Open = true;
                    }
                });
        }

        public int[,] GameBoard { get { return this.m_GameBoard; } }

        public string[,] GameBoardMeta { get { return this.m_GameBoardMeta; } }

        public List<IEntity> Entities { get; private set; }

        public void InitiateResetLevel()
        {
            this.m_NetworkAPI.SendMessage(
                "reset level",
                "");

            this.HandleResetLevel();
        }

        public void InitiateNextLevel()
        {
            this.m_NetworkAPI.SendMessage(
                "next level",
                "");

            this.HandleNextLevel();
        }

        public void HandleNextLevel()
        {
            this.m_MoveToNextLevel = true;
        }

        public void HandleResetLevel()
        {
            this.m_MoveToSameLevel = true;
        }

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
                            case 3:
                                this.m_GameBoard[(int)levelTile.X, (int)levelTile.Y] = 3;
                                break;
                            case 4:
                                this.m_GameBoard[(int)levelTile.X, (int)levelTile.Y] = 5;
                                break;
                            case 5:
                                this.m_GameBoard[(int)levelTile.X, (int)levelTile.Y] = 5;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 1:
                        switch (levelTile.TX)
                        {
                            case 1:
                                this.m_GameBoard[(int)levelTile.X, (int)levelTile.Y] = 1;
                                break;
                            case 6:
                                this.m_GameBoard[(int)levelTile.X, (int)levelTile.Y] = 4;
                                break;
                            case 7:
                                this.m_GameBoard[(int)levelTile.X, (int)levelTile.Y] = 7;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 2:
                        switch (levelTile.TX)
                        {
                            case 2:
                                this.m_GameBoard[(int)levelTile.X, (int)levelTile.Y] = 2;
                                break;
                            case 4:
                                this.m_GameBoard[(int)levelTile.X, (int)levelTile.Y] = 0;
                                this.m_GameBoardMeta[(int)levelTile.X, (int)levelTile.Y] = "death";
                                break;
                            case 7:
                                this.m_GameBoard[(int)levelTile.X, (int)levelTile.Y] = 1;
                                this.m_GameBoardMeta[(int)levelTile.X, (int)levelTile.Y] = "end";
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            this.Entities.RemoveAll(x => x is LevelTileEntity);

            foreach (var entity in this.Entities.OfType<BaseNetworkEntity>())
            {
                if (this.m_NetworkAPI.WasJoin)
                {
                    entity.LocallyOwned = entity.JoinShouldOwn;
                }
                else
                {
                    entity.LocallyOwned = !entity.JoinShouldOwn;
                }
            }

            foreach (var entity in this.Entities.OfType<SpawnEntity>().ToList())
            {
                switch (entity.Name)
                {
                    case "RedSpawn":
                    case "BlueSpawn":
                        var check = entity.Name == "RedSpawn" ? this.m_NetworkAPI.WasJoin : !this.m_NetworkAPI.WasJoin;

                        if (check)
                        {
                            this.m_MyPlayer = this.m_EntityFactory.CreatePlayerEntity(this.m_NetworkAPI.WasJoin, true);
                            this.m_MyPlayer.X = entity.X + 0.25f;
                            this.m_MyPlayer.Y = 1f;
                            this.m_MyPlayer.Z = entity.Z + 0.25f;
                            this.Entities.Add(this.m_MyPlayer);
                        }
                        else
                        {
                            this.m_OtherPlayer = this.m_EntityFactory.CreatePlayerEntity(!this.m_NetworkAPI.WasJoin, false);
                            this.m_OtherPlayer.X = entity.X + 0.25f;
                            this.m_OtherPlayer.Y = 1f;
                            this.m_OtherPlayer.Z = entity.Z + 0.25f;
                            this.Entities.Add(this.m_OtherPlayer);
                        }
                        break;
                }
            }

            this.Entities.RemoveAll(x => x is SpawnEntity);
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
                        if (this.m_GameBoard[x, y] == 0)
                        {
                            continue;
                        }

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
                    new Vector2(renderContext.GraphicsDevice.Viewport.Width - 10, 10),
                    gameContext.FPS + " FPS",
                    this.m_DefaultFont,
                    horizontalAlignment: HorizontalAlignment.Right);

                var instructions = 
                    @"Up/Down/Left/Right - Move
Z - Jump
X - Pickup / Drop / Throw
R - Restart";

                var lines = instructions.Split('\n');
                var i = 0;
                foreach (var line in lines)
                {
                    this.m_2DRenderUtilities.RenderText(
                        renderContext,
                        new Vector2(10, 10 + 20 * i++),
                        line,
                        this.m_DefaultFont);
                }

                if (this.m_NetworkAPI.ClientDisconnectAccumulator > 0)
                {
                    this.m_2DRenderUtilities.RenderText(
                        renderContext,
                        new Vector2(renderContext.GraphicsDevice.Viewport.Width - 10, 30),
                        "Other player has disconnected (" + (this.m_NetworkAPI.ClientDisconnectAccumulator / 30f).ToString("F2") + "s)",
                        this.m_DefaultFont,
                        textColor: Color.Red,
                        horizontalAlignment: HorizontalAlignment.Right);
                }
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_NetworkAPI.Update();

            if (this.m_MoveToNextLevel)
            {
                gameContext.SwitchWorld<IWorldFactory>(x => x.CreateIntermissionWorld(this.m_CurrentLevel + 1));
                return;
            }

            if (this.m_MoveToSameLevel)
            {
                gameContext.SwitchWorld<IWorldFactory>(x => x.CreateIntermissionWorld(this.m_CurrentLevel));
                return;
            }

            if (this.m_NetworkAPI.Disconnected)
            {
                gameContext.Game.Exit();
            }
        }
    }
}
