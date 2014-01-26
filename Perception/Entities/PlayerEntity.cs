using System;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Perception
{
    public class PlayerEntity : BaseCollisionEntity
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

            this.LocallyOwned = locallyOwned;

            this.Width = 0.5f;
            this.Depth = 0.5f;

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

        public float InferredXSpeed
        {
            get;
            set;
        }

        public float InferredZSpeed
        {
            get;
            set;
        }

        public float LastX
        {
            get;
            set;
        }

        public float LastZ
        {
            get;
            set;
        }

        public BaseNetworkEntity HeldObject
        {
            get;
            private set;
        }

        public bool HoldingObject
        {
            get { return this.HeldObject != null; }
        }

        public void Pickup(BaseNetworkEntity entity)
        {
            this.m_NetworkAPI.SendMessage(
                "take object",
                entity.ID + "");

            entity.LocallyOwned = true;

            entity.X = this.X;
            entity.Y = this.Y;
            entity.Z = this.Z;

            this.HeldObject = entity;
        }

        public void Throw()
        {
            if (this.HeldObject == null)
            {
                return;
            }

            var entity = this.HeldObject;

            entity.XSpeed = -this.InferredXSpeed * 6f;
            entity.YSpeed = 0.5f + this.YSpeed;
            entity.ZSpeed = -this.InferredZSpeed * 6f;

            this.HeldObject = null;
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.InferredXSpeed = this.LastX - this.X;
            this.InferredZSpeed = this.LastZ - this.Z;

            if (this.LocallyOwned)
            {
                this.Y += this.YSpeed / 5f;

                this.YSpeed -= 0.05f;
                if (this.YSpeed < -2)
                {
                    this.YSpeed = -2;
                }

                this.AdjustHeight(gameContext);

                this.m_NetworkAPI.SendMessage(
                    "player update",
                    this.X + "|" + this.Y + "|" + this.Z);

                if (this.HeldObject != null)
                {
                    if (!this.HeldObject.LocallyOwned)
                    {
                        this.Throw();
                    }
                    else
                    {
                        this.HeldObject.X = this.X + 0.25f;
                        this.HeldObject.Y = this.Y + 0.25f;
                        this.HeldObject.Z = this.Z + 0.25f;

                        this.HeldObject.XSpeed = 0;
                        this.HeldObject.YSpeed = 0;
                        this.HeldObject.ZSpeed = 0;
                    }
                }

                if (this.IsOnFloor(gameContext))
                {
                    this.HandleMeta(gameContext);
                }
            }

            this.LastX = this.X;
            this.LastZ = this.Z;
        }

        public bool CheckAndImpact(IGameContext gameContext, float relX, float relY, float relZ)
        {
            // called when the player has moved and we need to impact any crates that
            // might be in our way
            foreach (var crate in gameContext.World.Entities.OfType<CrateEntity>().Where(
                a => !(this.X + relX > a.X + a.Width || this.X + relX + this.Width < a.X || 
                    this.Z + relZ > a.Z + a.Depth || this.Z + relZ + this.Depth < a.Z)))
            {
                if (this.Y > crate.Y + 0.9f)
                {
                    continue;
                }

                if (!crate.LocallyOwned)
                {
                    this.m_NetworkAPI.SendMessage(
                        "take object",
                        crate.ID + "");
                    crate.LocallyOwned = true;
                }

                if (crate.CanMoveTo(gameContext, crate.X + relX, crate.Z + relZ))
                {
                    crate.X += relX;
                    crate.Z += relZ;

                    if (!this.CanMoveTo(gameContext, this.X + relX, this.Z + relZ))
                    {
                        // extra adjustment for floating point errors
                        crate.X += relX;
                        crate.Z += relZ;
                    }

                    return true;
                }

                return false;
            }

            return true;
        }

        private void HandleMeta(IGameContext gameContext)
        {
            var world = (PerceptionWorld)gameContext.World;

            var meta = world.GameBoardMeta[(int)Math.Round(this.X - 0.5f), (int)Math.Round(this.Z - 0.5f)];

            if (meta == null)
            {
                return;
            }

            switch (meta)
            {
                case "death":
                    if (this.Y <= 0)
                    {
                        world.InitiateResetLevel();
                    }

                    break;
            }
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                this.m_CubeRenderer.RenderCube(
                    renderContext,
                    Matrix.CreateScale(0.5f) *
                    Matrix.CreateTranslation(new Vector3(this.X, this.Y, this.Z)),
                    this.m_PlayerTexture,
                    new Vector2(0, 0),
                    new Vector2(1, 1));
            }
            else
            {
                var world = (PerceptionWorld)gameContext.World;

                try
                {
                    var height = this.GetHeight(gameContext, this.X, this.Z);

                    if (height == 0 && this.Y < 0)
                    {
                        // draw fade out
                        var fadeValue = (-(-10 - this.Y) / 10f);

                        var color = new Color(0f, 0f, 0f, 1f - fadeValue);

                        this.m_2DRenderUtilities.RenderRectangle(
                            renderContext,
                            new Rectangle(
                                0, 0,
                                renderContext.GraphicsDevice.Viewport.Width,
                                renderContext.GraphicsDevice.Viewport.Height),
                            color,
                            true);
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }
            }
        }
    }
}

