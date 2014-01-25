namespace Perception
{
    using Ninject;

    using Protogame;

    public class PerceptionGame : CoreGame<MenuWorld, Default3DWorldManager>
    {
        public PerceptionGame(StandardKernel kernel)
            : base(kernel)
        {
            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            this.Window.Title = "Perspective";
            this.GameContext.ResizeWindow(1024, 768);
        }
    }
}
