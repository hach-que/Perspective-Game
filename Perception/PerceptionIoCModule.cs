using System;
using Ninject.Modules;
using Ninject.Modules;
using Protogame;
using Ninject.Extensions.Factory;

namespace Perception
{
    public class PerceptionIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IEntityFactory>().ToFactory();

            this.Bind<ICubeRenderer>().To<DefaultCubeRenderer>();

            this.Bind<IEventBinder<IGameContext>>().To<PerceptionEventBinder>();
        }
    }
}

