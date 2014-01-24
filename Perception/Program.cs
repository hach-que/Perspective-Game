#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX

namespace Perception
{
    using Ninject;

    using Protogame;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame3DIoCModule>();
            kernel.Load<ProtogameAssetIoCModule>();
            kernel.Load<ProtogameEventsIoCModule>();
            kernel.Load<PerceptionIoCModule>();
            AssetManagerClient.AcceptArgumentsAndSetup<GameAssetManagerProvider>(kernel, args);

            using (var game = new PerceptionGame(kernel))
            {
                game.Run();
            }
        }
    }
}

#endif
