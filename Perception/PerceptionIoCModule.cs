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
            this.Bind<IWorldFactory>().ToFactory();

            this.Bind<ICubeRenderer>().To<DefaultCubeRenderer>();

            this.Bind<IEventBinder<IGameContext>>().To<PerceptionEventBinder>();

            this.Bind<ISkin>().To<BasicSkin>();
            this.Bind<IBasicSkin>().To<PerceptionBasicSkin>();

            this.Bind<ITileEntity>().To<LevelTileEntity>().Named("NewTileset0");

            this.Bind<IEntity>().To<KeyEntity>().Named("Key");
            this.Bind<IEntity>().To<DoorEntity>().Named("Door");
            this.Bind<IEntity>().To<SpawnEntity>().Named("RedSpawn");
            this.Bind<IEntity>().To<SpawnEntity>().Named("BlueSpawn");
        }
    }
}

