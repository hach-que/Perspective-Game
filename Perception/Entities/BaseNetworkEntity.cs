using System;
using Protogame;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Perception
{
    public class BaseNetworkEntity : BaseCollisionEntity
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

        public bool JoinShouldOwn
        {
            get;
            protected set;
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

        public float XSpeed
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

        public bool CanPush
        {
            get;
            set;
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (this.LocallyOwned)
            {
                if (this.CanMoveTo(gameContext, this.X + this.XSpeed / 5f, this.Z + this.ZSpeed / 5f))
                {
                    this.X += this.XSpeed / 5f;
                    this.Y += this.YSpeed / 5f;
                    this.Z += this.ZSpeed / 5f;
                }

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

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
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

