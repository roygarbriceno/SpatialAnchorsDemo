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
        public AnchorsFragment()
        {
        }
     
        protected override Config GetSessionConfiguration(Session session)
        {
            var config = new Config(session);
            config.SetUpdateMode(Config.UpdateMode.LatestCameraImage);
            config.SetFocusMode(Config.FocusMode.Auto);                     
            ArSceneView.SetupSession(session);
            return config;
        }
    }
}