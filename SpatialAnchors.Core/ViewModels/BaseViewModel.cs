namespace SpatialAnchors.Core.ViewModels
{
    using System;
    using MvvmCross;
    using MvvmCross.Localization;
    using MvvmCross.ViewModels;
    using System.Threading.Tasks;
    using MvvmCross.Logging;
    using MvvmCross.Navigation;
    using Interfaces;
    using MvvmCross.Plugin.JsonLocalization;
    using Services;
    using MvvmCross.Plugin.Messenger;
    using System.Text;
    using System.Linq;
    using Xamarin.Essentials;


    /// <summary>
    /// Base class for all our ViewModels
    /// </summary>
    public abstract class BaseViewModel : MvxNavigationViewModel
    {
        private bool isBusy;
        private readonly IMvxTextProviderBuilder textProviderBuilder;


        /// <summary>
        /// Page title 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Current messenger service
        /// </summary>
        protected IMvxMessenger MessengerService { get; }



        /// <summary>
        /// Get's if the viewModel is busy doing something
        /// </summary>
        public bool IsBusy
        {
            get => this.isBusy;
            set
            {
                this.isBusy = value;
                RaisePropertyChanged(() => IsBusy);
                RaisePropertyChanged(() => IsEnabled);
            }
        }


        /// <summary>
        /// Gets if the ViewModel is enabled.
        /// It's the inverse of IsBusy for easier binding. (If IsBusy = true them IsEnabled = false)
        /// </summary>
        public bool IsEnabled
        {
            get => !this.isBusy;
            set
            {
                this.isBusy = !value;
                RaisePropertyChanged(() => IsEnabled);
            }
        }


       
        /// <summary>
        /// Platform service
        /// </summary>
        protected IPlatformService PlatformService { get; }


        /// <summary>
        /// Notification service
        /// </summary>
        protected INotificationService NotificationService { get; }


        /// <summary>
        /// DATA service
        /// </summary>
        protected IDataService DataService { get; }


        /// <summary>
        /// Source for localized texts
        /// </summary>
        public IMvxLanguageBinder TextSource =>
            new MvxLanguageBinder(TextProviderConstants.GeneralNamespace, TextProviderConstants.ClassName);


        /// <summary>
        /// Helper method for getting a localized text
        /// </summary>
        /// <param name="text">Text to get</param>
        /// <returns>Localized text</returns>
        public string GetText(string text)
        {
            return this.textProviderBuilder.TextProvider.GetText(
                TextProviderConstants.GeneralNamespace, TextProviderConstants.ClassName, text);
        }


        /// <summary>
        /// Converts a DateTime into a TimeSpan
        /// </summary>
        public static TimeSpan FromDate(DateTime date)
        {
            return new TimeSpan(date.Hour, date.Minute, date.Second);
        }


        /// <summary>
        /// Converts a TimeStan into a DateTime
        /// </summary>
        public static DateTime FromTimeSpan(TimeSpan timespan)
        {
            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, timespan.Hours,
                timespan.Minutes, timespan.Seconds);
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logProvider"></param>
        /// <param name="navigationService"></param>
        protected BaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            this.isBusy = false;
            this.textProviderBuilder = Mvx.IoCProvider.GetSingleton<IMvxTextProviderBuilder>();
            this.MessengerService = Mvx.IoCProvider.GetSingleton<IMvxMessenger>();
            this.PlatformService = Mvx.IoCProvider.GetSingleton<IPlatformService>();
            this.DataService = Mvx.IoCProvider.GetSingleton<IDataService>();
        }



        public Task LogExceptionAsync(Exception ex)
        {
            return Task.FromResult(true);
        }
    }


    /// <summary>
    /// Base ViewModel with parameters
    /// </summary>
    public abstract class BaseViewModel<TParameter> : BaseViewModel, IMvxViewModel<TParameter> where TParameter : class
    {
        protected BaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
        }

        /// <summary>
        /// Called with the ViewModel's parameters
        /// </summary>
        public abstract void Prepare(TParameter parameter);
    }


    /// <summary>
    /// Base ViewModel with parameters and result
    /// </summary>
    public abstract class BaseViewModel<TParameter, TResult> : BaseViewModel, IMvxViewModel<TParameter, TResult>
        where TParameter : class
        where TResult : class
    {
        protected BaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
        }

        /// <summary>
        /// Called with the ViewModel's parameters
        /// </summary>
        public abstract void Prepare(TParameter parameter);

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }
    }


    /// <summary>
    /// Base ViewModel with parameters and result
    /// </summary>
    public abstract class BaseViewModelResult<TResult> : BaseViewModel, IMvxViewModelResult<TResult>
        where TResult : class
    {
        protected BaseViewModelResult(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
        }

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            if (viewFinishing && CloseCompletionSource != null && !CloseCompletionSource.Task.IsCompleted && !CloseCompletionSource.Task.IsFaulted)
                CloseCompletionSource?.TrySetCanceled();

            base.ViewDestroy(viewFinishing);
        }
    }
}
