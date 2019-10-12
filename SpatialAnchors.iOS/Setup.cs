namespace SpatialAnchors.iOS
{
    using MvvmCross.Forms.Platforms.Ios.Core;
    using MvvmCross.Logging;
    using SpatialAnchors.iOS.Services;
    using MvvmCross;
    using MvvmCross.Base;
    using MvvmCross.Plugin.Json;
    using SpatialAnchors.Core;
    using SpatialAnchors.Core.Interfaces;


    /// <summary>
    /// iOS setup class
    /// </summary>
    public class Setup : MvxFormsIosSetup<MvxApp, App>
    {
        /// <summary>
        /// Sets the log provider
        /// </summary>
        /// <returns></returns>
        public override MvxLogProviderType GetDefaultLogProviderType()
            => MvxLogProviderType.Console;


        /// <summary>
        /// Initializes the platform services
        /// </summary>
        protected override void InitializeFirstChance()
        {
            Mvx.IoCProvider.RegisterSingleton<IMvxJsonConverter>(new MvxJsonConverter());
            Mvx.IoCProvider.RegisterSingleton<ISpatialAnchorsService>(new SpatialAnchorsService());
            base.InitializeFirstChance();

        }


    }
}