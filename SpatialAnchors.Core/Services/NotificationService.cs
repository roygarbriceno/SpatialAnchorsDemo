namespace SpatialAnchors.Core.Services
{
    using SpatialAnchors.Core.Interfaces;
    using System.Threading.Tasks;

    /// <summary>
    /// Notification service.
    /// </summary>
    public class NotificationService : INotificationService
    {
        /// <summary>
        /// Shows a pop up to the user (Xamarin.Forms DisplayAlert)
        /// </summary>
        public async Task NotifyAsync(string title, string message)
        {
            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(title, message, "Ok");
        }
    }
}

