namespace SpatialAnchors.Core.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Notification service.
    /// </summary>
    public interface INotificationService
    {
        Task NotifyAsync(string title, string message);

    }
}
