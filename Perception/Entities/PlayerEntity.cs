using System;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Perception
{
    public class PlayerEntity : IEntity
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly ICubeRenderer m_CubeRenderer;

        private readonly TextureAsset m_PlayerTexture;

        private readonly INetworkAPI m_NetworkAPI;

        public PlayerEntity(
            I2DRenderUtilities twodRenderUtilities,
            ICubeRenderer cubeRenderer,
            IAssetManagerProvider assetManagerProvider,
            INetworkAPI networkAPI,
            bool isRedColor,
            bool locallyOwned)
        {
            this.m_NetworkAPI = networkAPI;
            this.m_2DRenderUtilities = twodRenderUtilities;
            this.m_CubeRenderer = cubeRenderer;
            this.m_PlayerTexture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("texture." + (isRedColor ? "Red" : "Blue"));

            this.X = 5.5f;
            this.Y = 1f;
            this.Z = 5.5f;

            this.LocallyOwned = locallyOwned;

            if (!this.LocallyOwned)
            {
                networkAPI.ListenForMessage(
                    "player update",
                    a =>
                    {
                        var values = a.Split('|').Select(x => float.Parse(x)).ToArray();

                        this.X = values[0];
                        this.Y = values[1];
                        this.Z = values[2];
                    });
            }
        }

        public bool LocallyOwned
        {
            get;
            set;
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

        public float YSpeed
        {
            get;
            set;
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.Y += this.YSpeed / 5f;

            this.YSpeed -= 0.05f;
            if (this.YSpeed < -2)
            {
                this.YSpeed = -2;
            }

            this.AdjustHeight(gameContext);

            if (this.LocallyOwned)
            {
                this.m_NetworkAPI.SendMessage(
                    "player update",
                    this.X + "|" + this.Y + "|" + this.Z);
            }
        }

        public bool CanMoveTo(IGameContext gameContext, float x, float z)
        {
            var world = (PerceptionWorld)gameContext.World;
            try
            {
                var height = world.GameBoard[(int)Math.Round(x - 0.5f), (int)Math.Round(z - 0.5f)];

                return this.Y >= height;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        private void AdjustHeight(IGameContext gameContext)
        {
            var world = (PerceptionWorld)gameContext.World;
            var height = world.GameBoard[(int)Math.Round(this.X - 0.5f), (int)Math.Round(this.Z - 0.5f)];

            if (this.Y < height)
            {
                this.Y = height;
                this.YSpeed = 0;
            }
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

