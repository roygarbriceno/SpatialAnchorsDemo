namespace SpatialAnchors.Core.ViewModels
{
    using MvvmCross.Commands;
    using MvvmCross.Logging;
    using MvvmCross.Navigation;
    using MvvmCross.ViewModels;
    using SpatialAnchors.Core.Arguments;
    using SpatialAnchors.Core.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// AR Logic
    /// </summary>
    public class AnchorsViewModel : BaseViewModel<SpatialAnchorsParameter>
    {
        private IMvxCommand captureChallengeCommand;

        /// <summary>
        /// 
        /// </summary>
        public SpatialAnchorsParameter Parameters { get; private set; }


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


        /// <summary>
        /// Receive the viewmodels parameters
        /// </summary>        
        public override void Prepare(SpatialAnchorsParameter parameter)
        {
            this.Parameters = parameter;
        }

    }
}
