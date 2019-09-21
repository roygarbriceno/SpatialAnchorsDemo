namespace SpatialAnchors.Core.ViewModels
{
    using MvvmCross.Commands;
    using MvvmCross.Logging;
    using MvvmCross.Navigation;


    /// <summary>
    /// Main view
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private IMvxCommand showArCommand;


        /// <summary>
        /// Shows the AR view
        /// </summary>
        public IMvxCommand ShowArCommand => showArCommand ?? (showArCommand = new MvxCommand(() =>
        {
            this.NavigationService.Navigate<AnchorsViewModel>();
        }));


        /// <summary>
        /// Gets by DI the required services
        /// </summary>
        public MainViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
        }
    }
}
