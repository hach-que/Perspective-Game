using System;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Perception
{
    public class KeyEntity : BaseNetworkEntity
    {
        public KeyEntity(
            I2DRenderUtilities twodRenderUtilities,
            ICubeRenderer cubeRenderer,
            IAssetManagerProvider assetManagerProvider,
            INetworkAPI networkAPI,
            int id,
            bool locallyOwned)
            : base(twodRenderUtilities,
                cubeRenderer,
                assetManagerProvider,
                networkAPI,
                id,
                locallyOwned)
        {
        }
    }
}

