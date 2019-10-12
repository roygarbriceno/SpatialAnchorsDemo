namespace SpatialAnchors.Core.ViewModels
{    
    using MvvmCross.Logging;
    using MvvmCross.Navigation; 
    using SpatialAnchors.Core.Arguments;
    using SpatialAnchors.Core.Interfaces;
    using System;
    using System.Linq;
    

    /// <summary>
    /// AR Logic
    /// </summary>
    public class AnchorsViewModel : BaseViewModel<SpatialAnchorsParameter>
    {
        private ISpatialAnchorsService spatialAnchorsService;
        private SpatialAnchorsParameter parameters;

    
        /// <summary>
        /// Current mode of the session
        /// </summary>
        public SpatialAnchorsMode Mode { get => this.spatialAnchorsService.Mode; } 


        /// <summary>
        /// Current status of the session
        /// </summary>
        public SpatialAnchorStatus Status { get => this.spatialAnchorsService.Status; }


        /// <summary>
        /// Gets by DI the required services
        /// </summary>
        public AnchorsViewModel(IMvxLogProvider logProvider,
            IMvxNavigationService navigationService, ISpatialAnchorsService spatialAnchorsService) : base(logProvider, navigationService)
        {
            this.spatialAnchorsService = spatialAnchorsService;

            this.spatialAnchorsService.SaveAnchor += async (sender, args) =>
            {
                try
                {
                    await this.DataService.SaveAnchorAsync(args);
                }
                catch (Exception ex)
                {
                    ShowMessage("Error", "ErrorSavingAnchor", ex.Message);
                }
            };

            this.spatialAnchorsService.ShowMessage += (sender, args) =>
            {
                ShowMessage("Info", args);
            };
        }


        /// <summary>
        /// Receive the viewmodels parameters
        /// </summary>        
        public override void Prepare(SpatialAnchorsParameter parameter)
        {
            this.parameters = parameter;
        }


        /// <summary>
        /// Starts the AR session
        /// </summary>        
        public void StartSession(object context, object scene)
        {
            this.spatialAnchorsService.StartSession(context, scene);
            this.spatialAnchorsService.LoadModels();

            if (this.parameters.Mode == SpatialAnchorsMode.SearchAnchors)
            {
                this.spatialAnchorsService.StartLocatingAnchors(this.parameters.Anchors.Select(x=>x.AnchorId).ToArray());
                ShowMessage("Info", "StartLocatingAnchors");
            }
            else
            {
                ShowMessage("Info", "StartAddingAnchors");
            }
        }


        /// <summary>
        /// Updates a frame in the AR session
        /// </summary>        
        public void ProcessFrame(object frame)
        {
            this.spatialAnchorsService.ProcessFrame(frame);
        }



        /// <summary>
        /// Stops the AR session 
        /// </summary>        
        public void StopSession()
        {
            this.spatialAnchorsService.StopSession();
        }


        /// <summary>
        /// Shows a message to the user
        /// </summary>        
        public void ShowMessage(string title, string text, string details = "")
        {
            InvokeOnMainThread(() =>
            {
                var message = $"{GetText(text)} {details}";
                this.NotificationService.NotifyAsync(GetText(title), message);
            });
        }
    }
}
