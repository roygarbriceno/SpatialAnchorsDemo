namespace SpatialAnchors.Core.ViewModels
{
    using System;
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
            this.NavigationService.Navigate<AnchorsViewModel, SpatialAnchorsParameter>(new SpatialAnchorsParameter { Mode = SpatialAnchorsMode.AddAnchors });
        }));


        /// <summary>
        /// Shows the AR view for searching anchors
        /// </summary>
        public IMvxCommand SearchAnchorsCommand => searchAnchorsCommand ?? (searchAnchorsCommand = new MvxCommand(async () =>
        {

            try
            {
                this.IsBusy = true;
                var anchors = await this.DataService.GetAnchorsAsync();
                await this.NavigationService.Navigate<AnchorsViewModel, SpatialAnchorsParameter>(new SpatialAnchorsParameter
                {
                    Mode = SpatialAnchorsMode.SearchAnchors,
                    Anchors = anchors.ToArray()
                });
            }
            catch (Exception ex)
            {
                // Do something
            }
            finally
            {
                this.IsBusy = false;
            }          
        }));


        /// <summary>
        /// Gets by DI the required services
        /// </summary>
        public MainViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
        }
    }
}
