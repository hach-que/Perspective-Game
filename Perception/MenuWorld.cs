using System;
using Protogame;
using System.Collections.Generic;
using System.Net;
using Microsoft.Xna.Framework;

namespace Perception
{
    public class MenuWorld : IWorld
    {
        private IGameContext m_LastGameContext;

        private TextureAsset m_LogoTexture;

        private I2DRenderUtilities m_2DRenderUtilities;

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                this.m_2DRenderUtilities.RenderTexture(
                    renderContext,
                    new Vector2(0, 0),
                    this.m_LogoTexture,
                    size: new Vector2(600, 100));
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_LastGameContext = gameContext;
        }

        public System.Collections.Generic.List<IEntity> Entities
        {
            get;
            private set;
        }

        public void Dispose()
        {
        }

        private readonly IWorldFactory m_WorldFactory;

        public MenuWorld(ISkin skin, IWorldFactory worldFactory, IAssetManagerProvider assetManagerProvider, I2DRenderUtilities twodRenderUtilities)
        {
            this.m_2DRenderUtilities = twodRenderUtilities;

            this.m_LogoTexture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("texture.Logo");

            this.m_WorldFactory = worldFactory;

            this.Entities = new List<IEntity>();

            var startServer = new Button();
            startServer.Text = "Start Server";
            startServer.Click += (sender, e) =>
            {
                this.m_LastGameContext.SwitchWorld<IWorldFactory>(
                    x => x.CreateLobbyWorld(false, IPAddress.Any));
            };

            var ipAddressTextBox = new TextBox();

            var joinGame = new Button();
            joinGame.Text = "Join Game";
            joinGame.Click += (sender, e) =>
            {
                this.m_LastGameContext.SwitchWorld<IWorldFactory>(
                    x => x.CreateLobbyWorld(true, IPAddress.Parse(ipAddressTextBox.Text)));
            };

            var exitGame = new Button();
            exitGame.Text = "Exit Game";
            exitGame.Click += (sender, e) =>
            {
                this.m_LastGameContext.Game.Exit();
            };

            var vertical = new VerticalContainer();
            vertical.AddChild(new EmptyContainer(), "*");
            vertical.AddChild(new Label { Text = "" }, "25");
            vertical.AddChild(new EmptyContainer(), "*");
            vertical.AddChild(startServer, "25");
            vertical.AddChild(new EmptyContainer(), "*");
            vertical.AddChild(new Label { Text = "Server IP address:" }, "20");
            vertical.AddChild(ipAddressTextBox, "20");
            vertical.AddChild(joinGame, "25");
            vertical.AddChild(new EmptyContainer(), "*");
            vertical.AddChild(exitGame, "25");
            vertical.AddChild(new EmptyContainer(), "*");

            var horizontal = new HorizontalContainer();
            horizontal.AddChild(new EmptyContainer(), "*");
            horizontal.AddChild(vertical, "250");
            horizontal.AddChild(new EmptyContainer(), "*");

            var canvas = new Canvas();
            canvas.SetChild(horizontal);

            this.Entities.Add(new CanvasEntity(skin, canvas));
        }
    }
}

