namespace SpatialAnchors.Core.ViewModels
{
    using MvvmCross.Commands;
    using MvvmCross.Logging;
    using MvvmCross.Navigation;
    using SpatialAnchors.Core.Arguments;


    /// <summary>
    /// Main view
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private IMvxCommand createAnchorsCommand;
        private IMvxCommand searchAnchorsCommand;


        /// <summary>
        /// Shows the AR view for creating new anchors
        /// </summary>
        public IMvxCommand CreateAnchorsCommand => createAnchorsCommand ?? (createAnchorsCommand = new MvxCommand(() =>
        {
            this.NavigationService.Navigate<AnchorsViewModel, SpatialAnchorsParameter>(new SpatialAnchorsParameter { Mode = SpatialAnchorsMode.Adding });
        }));


        /// <summary>
        /// Shows the AR view for searching anchors
        /// </summary>
        public IMvxCommand SearchAnchorsCommand => searchAnchorsCommand ?? (searchAnchorsCommand = new MvxCommand(() =>
        {
            this.NavigationService.Navigate<AnchorsViewModel, SpatialAnchorsParameter>(new SpatialAnchorsParameter { Mode = SpatialAnchorsMode.Searching  });
        }));


        /// <summary>
        /// Gets by DI the required services
        /// </summary>
        public MainViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
        }
    }
}
