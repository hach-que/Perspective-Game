using System;
using Protogame;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Perception
{
    public class BaseNetworkEntity : IEntity
    {
        protected readonly I2DRenderUtilities m_2DRenderUtilities;

        protected readonly ICubeRenderer m_CubeRenderer;

        protected readonly INetworkAPI m_NetworkAPI;

        public BaseNetworkEntity(
            I2DRenderUtilities twodRenderUtilities,
            ICubeRenderer cubeRenderer,
            IAssetManagerProvider assetManagerProvider,
            INetworkAPI networkAPI,
            int id)
        {
            this.m_NetworkAPI = networkAPI;
            this.m_2DRenderUtilities = twodRenderUtilities;
            this.m_CubeRenderer = cubeRenderer;
            this.LocallyOwned = true;

            this.ID = id;

            networkAPI.ListenForMessage(
                "entity update",
                a =>
                {
                    if (!this.LocallyOwned)
                    {
                        var values = a.Split('|').Select(x => float.Parse(x)).ToArray();

                        if ((int)values[0] == id)
                        {
                            this.X = values[1];
                            this.Y = values[2];
                            this.Z = values[3];
                        }
                    }
                });

            networkAPI.ListenForMessage(
                "take object",
                a =>
                {
                    if (this.LocallyOwned)
                    {
                        var values = a.Split('|').Select(x => float.Parse(x)).ToArray();

                        if ((int)values[0] == id)
                        {
                            // other player is now owning this object
                            this.LocallyOwned = false;
                        }
                    }
                });
        }

        public int ID
        {
            get;
            private set;
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

        public float XSpeed
        {
            get;
            set;
        }

        public float YSpeed
        {
            get;
            set;
        }

        public float ZSpeed
        {
            get;
            set;
        }

        public bool CanPickup
        {
            get;
            set;
        }

        public virtual void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (this.LocallyOwned)
            {
                this.X += this.XSpeed / 5f;
                this.Y += this.YSpeed / 5f;
                this.Z += this.ZSpeed / 5f;

                if (Math.Abs(this.XSpeed) <= 0.05f)
                {
                    this.XSpeed = 0;
                }
                else
                {
                    if (this.XSpeed > 0)
                    {
                        this.XSpeed -= 0.01f;
                    }
                    else
                    {
                        this.XSpeed += 0.01f;
                    }
                }

                if (Math.Abs(this.ZSpeed) <= 0.05f)
                {
                    this.ZSpeed = 0;
                }
                else
                {
                    if (this.ZSpeed > 0)
                    {
                        this.ZSpeed -= 0.01f;
                    }
                    else
                    {
                        this.ZSpeed += 0.01f;
                    }
                }

                this.YSpeed -= 0.05f;

                if (this.YSpeed < -2)
                {
                    this.YSpeed = -2;
                }

                this.AdjustHeight(gameContext);

                this.m_NetworkAPI.SendMessage(
                    "entity update",
                    this.ID + "|" + this.X + "|" + this.Y + "|" + this.Z);
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

        public bool IsOnFloor(IGameContext gameContext)
        {
            var world = (PerceptionWorld)gameContext.World;
            try
            {
                var height = world.GameBoard[(int)Math.Round(this.X - 0.5f), (int)Math.Round(this.Z - 0.5f)];

                return this.Y == height;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        private void AdjustHeight(IGameContext gameContext)
        {
            var world = (PerceptionWorld)gameContext.World;
            var height = 0;
            try
            {
                height = world.GameBoard[(int)Math.Round(this.X - 0.5f), (int)Math.Round(this.Z - 0.5f)];
            }
            catch (IndexOutOfRangeException)
            {
                height = 0;
            }

            if (this.Y < height)
            {
                this.Y = height;
                this.YSpeed = 0;
            }
        }

        public virtual void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                this.m_CubeRenderer.RenderCube(
                    renderContext,
                    Matrix.CreateScale(0.5f) *
                    Matrix.CreateTranslation(new Vector3(this.X - 0.25f, this.Y, this.Z - 0.25f)),
                    new TextureAsset(renderContext.SingleWhitePixel),
                    new Vector2(0, 0),
                    new Vector2(1, 1));
            }
            else
            {
            }
        }
    }
}

