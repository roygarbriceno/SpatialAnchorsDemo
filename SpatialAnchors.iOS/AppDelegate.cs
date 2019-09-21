namespace SpatialAnchors.iOS
{
    using Foundation;
    using MobileTemplate.Core;
    using MvvmCross.Forms.Platforms.Ios.Core;
    using UIKit;

    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : MvxFormsApplicationDelegate<Setup, MvxApp, App>
    {
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            return base.FinishedLaunching(application, launchOptions);
        }

        protected override void LoadFormsApplication()
        {
            base.LoadFormsApplication();
            global::Xamarin.Forms.Forms.Init();
        }
    }
}


