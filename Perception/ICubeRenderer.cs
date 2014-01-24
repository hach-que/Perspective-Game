using System;
using Microsoft.Xna.Framework;
using Protogame;

namespace Perception
{
    public interface ICubeRenderer
    {
        void RenderCube(IRenderContext renderContext, Matrix transform, TextureAsset texture, Vector2 topLeftUV, Vector2 bottomRightUV);
    }
}

