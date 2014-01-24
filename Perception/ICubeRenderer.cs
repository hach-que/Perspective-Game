using System;
using Microsoft.Xna.Framework;
using Protogame;

namespace Perception
{
    public interface ICubeRenderer
    {
        void RenderCube(IRenderContext renderContext, Matrix transform, TextureAsset texture);
    }
}

