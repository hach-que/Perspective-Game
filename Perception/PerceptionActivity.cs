#if PLATFORM_ANDROID

namespace Perception
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;
    using Android.Content.PM;
    
    using Microsoft.Xna.Framework;
    
    using Ninject;
    
    using Protogame;
  
    [Activity(
        Label = "Perception",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden,
        ScreenOrientation = ScreenOrientation.Landscape)]
    public class PerceptionActivity : AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogameAssetIoCModule>();
            AssetManagerClient.AcceptArgumentsAndSetup<GameAssetManagerProvider>(kernel, null);

            // Create our OpenGL view, and display it
            PerceptionGame.Activity = this;
            var g = new PerceptionGame(kernel);
            SetContentView(g.AndroidGameWindow);
            g.Run();
        }
    }
}

#endif
