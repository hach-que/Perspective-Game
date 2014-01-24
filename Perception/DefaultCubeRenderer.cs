using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protogame;

namespace Perception
{
    public class DefaultCubeRenderer : ICubeRenderer
    {
        public void RenderCube(IRenderContext renderContext, Matrix transform, TextureAsset texture, Vector2 topLeftUV, Vector2 bottomRightUV)
        {
            var vertexes = new[]
            {
                new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(-1, 0, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 0, 1), new Vector3(-1, 0, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 1, 0), new Vector3(-1, 0, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 1, 1), new Vector3(-1, 0, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 0), new Vector3(1, 0, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 1), new Vector3(1, 0, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, -1, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 0, 1), new Vector3(0, -1, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 1, 1), new Vector3(0, 1, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(new Vector3(1, 0, 0), new Vector3(0, -1, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 0, 1), new Vector3(0, -1, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 0), new Vector3(0, 1, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 1), new Vector3(0, 1, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(0, 0, -1), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 1, 0), new Vector3(0, 0, -1), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(0, 1, 1), new Vector3(0, 0, 1), new Vector2(topLeftUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 0), new Vector3(0, 0, -1), new Vector2(bottomRightUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(new Vector3(1, 1, 1), new Vector3(0, 0, 1), new Vector2(bottomRightUV.X, bottomRightUV.Y)),
            };

            var indicies = new short[]
            {
                0, 2, 1,
                3, 1, 2,

                4, 5, 6, 
                7, 6, 5,

                0 + 8, 1 + 8, 4 + 8,
                5 + 8, 4 + 8, 1 + 8,

                2 + 8, 6 + 8, 3 + 8,
                7 + 8, 3 + 8, 6 + 8,

                0 + 16, 4 + 16, 2 + 16,
                6 + 16, 2 + 16, 4 + 16,

                1 + 16, 3 + 16, 5 + 16,
                7 + 16, 5 + 16, 3 + 16
            };

            renderContext.EnableTextures();
            renderContext.SetActiveTexture(texture.Texture);

            var world = renderContext.World;

            renderContext.World = transform;

            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    vertexes,
                    0,
                    vertexes.Length,
                    indicies,
                    0,
                    indicies.Length / 3);
            }

            renderContext.World = world;
        }
    }
}

