namespace SpatialAnchors.Droid
{
    using Core;
    using MvvmCross.Forms.Platforms.Android.Core;
    using MvvmCross.Logging;
    using MvvmCross;
    using MvvmCross.Forms.Presenters;
    using Core.Interfaces;
    using Services;

    /// <summary>
    /// Android setup class
    /// </summary>
    public class Setup : MvxFormsAndroidSetup<MvxApp, App>
    {
        /// <summary>
        /// Sets the log provider
        /// </summary>
        /// <returns></returns>
        public override MvxLogProviderType GetDefaultLogProviderType()
            => MvxLogProviderType.Console;


        /// <summary>
        /// Register the form presenter (MvvmCross)
        /// </summary>        
        protected override IMvxFormsPagePresenter CreateFormsPagePresenter(IMvxFormsViewPresenter viewPresenter)
        {
            var formsPresenter = base.CreateFormsPagePresenter(viewPresenter);
            Mvx.IoCProvider.RegisterSingleton(formsPresenter);
            return formsPresenter;
        }


        /// <summary>
        /// Initializes the platform services
        /// </summary>
        protected override void InitializeFirstChance()
        {
            Mvx.IoCProvider.RegisterSingleton<IPlatformService>(new PlatformService());
            base.InitializeFirstChance();
        }
    }
}