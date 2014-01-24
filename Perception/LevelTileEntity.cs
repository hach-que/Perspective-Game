using System;
using Protogame;

namespace Perception
{
    public class LevelTileEntity : ITileEntity
    {
        public LevelTileEntity(float x, float y, int tx, int ty)
        {
            this.X = x;
            this.Y = y;
            this.TX = tx;
            this.TY = ty;
            this.Width = 16;
            this.Height = 16;
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public int TX
        {
            get;
            set;
        }

        public int TY
        {
            get;
            set;
        }

        public TilesetAsset Tileset
        {
            get;
            set;
        }

        public float Width
        {
            get;
            set;
        }

        public float Height
        {
            get;
            set;
        }

        public float Depth
        {
            get;
            set;
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

