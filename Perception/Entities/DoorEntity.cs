using System;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

namespace Perception
{
    public class DoorEntity : BaseNetworkEntity
    {
        private ModelAsset m_DoorClosedModel;

        private ModelAsset m_DoorOpenModel;

        public DoorEntity(
            I2DRenderUtilities twodRenderUtilities,
            ICubeRenderer cubeRenderer,
            IAssetManagerProvider assetManagerProvider,
            INetworkAPI networkAPI,
            int id,
            int x,
            int y,
            Dictionary<string, string> attributes)
            : base(twodRenderUtilities,
                cubeRenderer,
                assetManagerProvider,
                networkAPI,
                Convert.ToInt32(attributes["NetworkID"]))
        {
            this.X = x / 16f + 0.5f;
            this.Z = y / 16f + 0.5f;
            this.JoinShouldOwn = Convert.ToBoolean(attributes["JoinOwns"]);
            this.CanPickup = false;

            //this.m_DoorClosedModel = assetManagerProvider.GetAssetManager().Get<ModelAsset>("model.DoorClosed");
            //this.m_DoorOpenModel = assetManagerProvider.GetAssetManager().Get<ModelAsset>("model.DoorOpen");
        }

        public bool Open
        {
            get;
            set;
        }

        public override void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            base.Update(gameContext, updateContext);

            if (this.Open)
            {
                return;
            }

            var players = gameContext.World.Entities.OfType<PlayerEntity>();

            foreach (var player in players.ToArray())
            {
                if (player.HoldingObject && player.HeldObject is KeyEntity)
                {
                    var target = new Vector3(this.X, this.Y, this.Z);
                    var source = new Vector3(player.X, player.Y, player.Z);

                    if ((target - source).Length() < 1)
                    {
                        this.Open = true;
                        var key = player.HeldObject;
                        player.Throw();

                        gameContext.World.Entities.Remove(key);

                        this.m_NetworkAPI.SendMessage(
                            "door unlock",
                            this.ID + "|" + key.ID);

                        return;
                    }
                }
            }
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            renderContext.SetActiveTexture(renderContext.SingleWhitePixel);

            if (!this.Open)
            {
                this.m_CubeRenderer.RenderCube(
                    renderContext,
                    Matrix.CreateTranslation(this.X - 0.5f, this.Y, this.Z - 0.5f),
                    new TextureAsset(renderContext.SingleWhitePixel),
                    new Vector2(0, 0),
                    new Vector2(0, 0));
            }

            /*var model = this.Open ? this.m_DoorOpenModel : this.m_DoorClosedModel;

            model.Draw(
                renderContext,
                Matrix.Identity,
                //Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * 
                //Matrix.CreateTranslation(this.X - 0.5f, this.Y, this.Z + 0.4f),
            model.AvailableAnimations.First().Name,
            TimeSpan.Zero);*/
        }
    }
}

