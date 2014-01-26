using System;
using Protogame;
using Microsoft.Xna.Framework;

namespace Perception
{
    public class PerceptionBasicSkin : IBasicSkin
    {
        public Color BackSurfaceColor
        {
            get
            {
                return new Color(0, 0, 0);
            }
        }

        public Color DarkEdgeColor
        {
            get
            {
                return new Color(32, 32, 32);
            }
        }

        public Color DarkSurfaceColor
        {
            get
            {
                return new Color(96, 96, 96);
            }
        }

        public Color LightEdgeColor
        {
            get
            {
                return new Color(160, 160, 160);
            }
        }

        public Color SurfaceColor
        {
            get
            {
                return new Color(128, 128, 128);
            }
        }

        public Color TextColor
        {
            get
            {
                return Color.Black;
            }
        }
    }
}

