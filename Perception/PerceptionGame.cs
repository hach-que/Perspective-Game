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
    }
}
