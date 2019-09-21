
namespace SpatialAnchors.Droid.Fragments
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
    using Google.AR.Sceneform.UX;

    using Google.AR.Core;    

    public class AnchorsFragment : ArFragment
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public AnchorsFragment()
        {
        }

        /// <summary>
        /// Overrise to setup the session configuration
        /// </summary>
        protected override Config GetSessionConfiguration(Session session)
        {
            //PlaneDiscoveryController.Hide();
            //PlaneDiscoveryController.SetInstructionView(null);
            // ArSceneView.PlaneRenderer.Enabled = false;

            var config = new Config(session);
            config.SetUpdateMode(Config.UpdateMode.LatestCameraImage);
            config.SetFocusMode(Config.FocusMode.Auto);

            //LoadImages(config, session);
            //config.AugmentedImageDatabase = this.imageDatabase;

            //session.Configure(config);
            ArSceneView.SetupSession(session);

            return config;
        }
    }
}