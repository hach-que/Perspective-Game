using System;
using Protogame;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Perception
{
    public abstract class BaseCollisionEntity : IEntity
    {
        public BaseCollisionEntity()
        {
        }

        public abstract void Update(IGameContext gameContext, IUpdateContext updateContext);

        public abstract void Render(IGameContext gameContext, IRenderContext renderContext);

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

        public float YSpeed
        {
            get;
            set;
        }

        public float Width
        {
            get;
            set;
        }

        public float Depth
        {
            get;
            set;
        }

        public Microsoft.Xna.Framework.BoundingBox GetBoundingBox()
        {
            return new Microsoft.Xna.Framework.BoundingBox(
                new Vector3(this.X, this.Y, this.Z),
                new Vector3(this.X + this.Width, this.Y + 1f, this.Z + this.Depth));
        }

        public bool CollidesWithObject(BaseCollisionEntity other)
        {
            return this.GetBoundingBox().Intersects(other.GetBoundingBox());
        }

        public bool CollidesWithAdjustedObject(BaseCollisionEntity other, float otherAbsX, float otherAbsY, float otherAbsZ)
        {
            return this.GetBoundingBox().Intersects(
                new Microsoft.Xna.Framework.BoundingBox(
                    new Vector3(otherAbsX, otherAbsY, otherAbsZ),
                    new Vector3(otherAbsX + this.Width - 0.01f, otherAbsY + 1f, otherAbsZ + this.Depth - 0.01f)));
        }

        public bool CollidesWithAdjustedObjectInfiniteY(BaseCollisionEntity other, float otherAbsX, float otherAbsZ)
        {
            return this.GetBoundingBox().Intersects(
                new Microsoft.Xna.Framework.BoundingBox(
                    new Vector3(otherAbsX, -100f, otherAbsZ),
                    new Vector3(otherAbsX + this.Width - 0.01f, 100f, otherAbsZ + this.Depth - 0.01f)));
        }

        public int GetHeight(IGameContext gameContext, float absX, float absZ, bool ignoreCrates = false)
        {
            var world = (PerceptionWorld)gameContext.World;
            var height = Math.Max(
                Math.Max(
                    world.GameBoard[(int)Math.Round(absX - 0.5f + 0.01f), (int)Math.Round(absZ - 0.5f + 0.01f)],
                    world.GameBoard[(int)Math.Round(absX + this.Width - 0.5f - 0.01f), (int)Math.Round(absZ - 0.5f + 0.01f)]),
                Math.Max(
                    world.GameBoard[(int)Math.Round(absX - 0.5f + 0.01f), (int)Math.Round(absZ + this.Depth - 0.5f - 0.01f)],
                    world.GameBoard[(int)Math.Round(absX + this.Width - 0.5f - 0.01f), (int)Math.Round(absZ + this.Depth - 0.5f - 0.01f)]));

            if (!ignoreCrates)
            {
                foreach (var crate in gameContext.World.Entities.OfType<CrateEntity>().Where(x => x.LocallyOwned || this.Y >= x.Y).Where(
                    a => !(absX > a.X + a.Width || absX + this.Width < a.X || absZ > a.Z + a.Depth || absZ + this.Depth < a.Z)))
                {
                    if (crate == this)
                    {
                        continue;
                    }

                    if (crate.Y >= height)
                    {
                        height = (int)(crate.Y + 1);
                    }
                }
            }

            return height;
        }

        public bool CanMoveTo(IGameContext gameContext, float absX, float absZ, bool ignoreCrates = false)
        {
            var world = (PerceptionWorld)gameContext.World;
            try
            {
                var height = this.GetHeight(gameContext, absX, absZ, ignoreCrates);

                // check if there are doors there
                var door = world.Entities.OfType<DoorEntity>().FirstOrDefault(a => a.CollidesWithAdjustedObjectInfiniteY(this, absX, absZ));
                if (door != null && !door.Open)
                {
                    return false;
                }

                if (!ignoreCrates)
                {
                    /* foreach (var crate in gameContext.World.Entities.OfType<CrateEntity>().Where(
                        a => !(absX > a.X + a.Width || absX + this.Width < a.X || absZ > a.Z + a.Depth || absZ + this.Depth < a.Z)))
                    {
                        if (crate == this)
                        {
                            continue;
                        }

                        return false;
                    }*/
                }

                return this.Y >= height;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public virtual void AdjustHeight(IGameContext gameContext)
        {
            var world = (PerceptionWorld)gameContext.World;
            var height = 0;
            try
            {
                height = this.GetHeight(gameContext, this.X, this.Z, this is CrateEntity);
            }
            catch (IndexOutOfRangeException)
            {
                height = 0;
            }

            if (this.Y < height && height != 0)
            {
                this.Y = height;
                this.YSpeed = 0;
            }
        }

        public bool IsOnFloor(IGameContext gameContext)
        {
            var world = (PerceptionWorld)gameContext.World;
            try
            {
                var height = this.GetHeight(gameContext, this.X, this.Z, this is CrateEntity);

                if (height == 0)
                {
                    return this.Y < -10;
                }

                return this.Y == height;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }
    }
}

