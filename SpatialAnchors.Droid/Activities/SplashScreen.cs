namespace SpatialAnchors.Droid.Activities
{
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content.PM;
    using Android.OS;
    using Core;
    using MvvmCross.Forms.Platforms.Android.Views;

    /// <summary>
    /// Splash Screen
    /// </summary>
    [Activity(
        Label = "@string/AppName"
        , MainLauncher = true
        , Icon = "@mipmap/ic_launcher"
        , Theme = "@style/Theme.Splash"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxFormsSplashScreenActivity<Setup, MvxApp, App>
    {
        /// <summary>
        /// Starts the main activity
        /// </summary>        
        protected override Task RunAppStartAsync(Bundle bundle)
        {
            StartActivity(typeof(MainActivity));
            return base.RunAppStartAsync(bundle);
        }
    }
}