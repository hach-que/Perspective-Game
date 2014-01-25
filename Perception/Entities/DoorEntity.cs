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
        private ModelAsset m_DoorModel;

        private ModelAsset m_DoorFrameModel;

        private int m_Rotation;

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

            this.m_Rotation = Convert.ToInt32(attributes["Rotation"]);

            this.m_DoorModel = assetManagerProvider.GetAssetManager().Get<ModelAsset>("model.Door");
            this.m_DoorFrameModel = assetManagerProvider.GetAssetManager().Get<ModelAsset>("model.DoorFrame");
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

            var midx = this.Open ? 90 : 0;

            this.m_DoorFrameModel.Draw(
                renderContext,
                Matrix.CreateScale(0.8f) * 
                Matrix.CreateTranslation(0f, 0f, 0.4f) *
                Matrix.CreateRotationY(MathHelper.ToRadians(this.m_Rotation)) *
                Matrix.CreateTranslation(this.X, this.Y, this.Z),
                this.m_DoorFrameModel.AvailableAnimations.First().Name,
                TimeSpan.Zero);

            this.m_DoorModel.Draw(
                renderContext,
                Matrix.CreateTranslation(0.5f, 0f, 0f) *
                Matrix.CreateRotationY(MathHelper.ToRadians(midx)) *
                Matrix.CreateTranslation(-0.5f, 0f, 0f) *
                Matrix.CreateScale(0.8f) * 
                Matrix.CreateTranslation(0f, 0f, 0.4f) *
                Matrix.CreateRotationY(MathHelper.ToRadians(this.m_Rotation)) *
                Matrix.CreateTranslation(this.X, this.Y, this.Z),
                this.m_DoorModel.AvailableAnimations.First().Name,
                TimeSpan.Zero);
        }
    }
}

