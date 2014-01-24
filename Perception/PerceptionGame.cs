namespace Perception
{
    using Ninject;

    using Protogame;

    public class PerceptionGame : CoreGame<PerceptionWorld, Default3DWorldManager>
    {
        public PerceptionGame(StandardKernel kernel)
            : base(kernel)
        {
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            this.Window.Title = "Perception";
            this.GameContext.ResizeWindow(1024, 768);
        }
    }
}
