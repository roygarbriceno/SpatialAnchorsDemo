namespace SpatialAnchors.Core.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    /// <summary>
    /// Notification service.
    /// </summary>
    public interface INotificationService
    {
        Page mainPage { get; set; }


        /// <summary>
        /// Shows a pop up to the user (Xamarin.Forms DisplayAlert)
        /// </summary>
        /// <returns></returns>
        Task<bool> NotifyAsync(string title, string message, string buttonText = "");


        /// <summary>
        /// Shows a pop up to the user with confirmation (Xamarin.Forms DisplayAlert)
        /// </summary>   
        Task<bool> ConfirmAsync(string title, string message, string yesText, string noText, Action<bool> callback);

    }
}
