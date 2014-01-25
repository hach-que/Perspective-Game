using System;
using Protogame;

namespace Perception
{
    public class SpawnEntity : IEntity
    {
        public string Name { get; set; }

        public SpawnEntity(
            string name,
            int x,
            int y)
        {
            this.Name = name;
            this.X = x / 16f;
            this.Z = y / 16f;
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        public float Z
        {
            get;
            set;
        }
    }
}

