namespace Perception
{
    using Ninject;

    using Protogame;

    public class PerceptionGame : CoreGame<PerceptionWorld, Default2DWorldManager>
    {
        public PerceptionGame(StandardKernel kernel)
            : base(kernel)
        {
        }
    }
}
