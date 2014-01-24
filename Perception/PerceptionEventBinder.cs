using System;
using Protogame;
using Microsoft.Xna.Framework.Input;

namespace Perception
{
    public class PerceptionEventBinder : StaticEventBinder<IGameContext>
    {
        public override void Configure()
        {
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.Left).To<MoveLeftAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.Right).To<MoveRightAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.Up).To<MoveUpAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.Down).To<MoveDownAction>();
            this.Bind<KeyPressEvent>(x => x.Key == Keys.Z).To<JumpAction>();
        }
    }
}

