namespace SpatialAnchors.Core.ViewModels
{
    using MvvmCross.Commands;
    using MvvmCross.Logging;
    using MvvmCross.Navigation;
    using MvvmCross.ViewModels;
    using SpatialAnchors.Core.Arguments;    
    using System;    
    using System.Threading.Tasks;


    /// <summary>
    /// AR Logic
    /// </summary>
    public class AnchorsViewModel : BaseViewModel<SpatialAnchorsParameter>
    {
        private IMvxCommand captureChallengeCommand;

        /// <summary>
        /// ViewModel parameters
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


     

        /// <summary>
        /// Saves an anchor
        /// </summary>        
        public async Task<bool> SaveAnchorAsync(SpatialAnchors.Models.Anchor anchor)
        {
            try
            {
                if (await this.DataService.SaveAnchorAsync(anchor))
                {
                    //this.Anchors.Add(anchor);
                    return true;
                }                
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        
    }
}
