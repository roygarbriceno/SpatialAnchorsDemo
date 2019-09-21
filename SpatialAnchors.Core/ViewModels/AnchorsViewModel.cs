namespace SpatialAnchors.Core.ViewModels
{
    using MvvmCross.Commands;
    using MvvmCross.Logging;
    using MvvmCross.Navigation;
    using MvvmCross.ViewModels;
    using SpatialAnchors.Core.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Text;


    /// <summary>
    /// AR Logic
    /// </summary>
    public class AnchorsViewModel : BaseViewModel
    {
        private IMvxCommand captureChallengeCommand;


        /// <summary>
        /// Request the image to captured for sending the challenge
        /// </summary>
        public IMvxCommand CaptureChallengeCommand => captureChallengeCommand ?? (captureChallengeCommand = new MvxCommand(() =>
        {
           // captureImageInteraction.Raise();
        }));


        /// <summary>
        /// Gets by DI the required services
        /// </summary>
        public AnchorsViewModel(IMvxLogProvider logProvider,
            IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {

        }

    }
}
