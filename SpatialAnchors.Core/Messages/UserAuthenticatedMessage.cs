namespace SpatialAnchors.Core.Messages
{
    using SpatialAnchors.Models;
    using MvvmCross.Plugin.Messenger;

    /// <summary>
    /// User authenticated
    /// </summary>
    public class UserAuthenticatedMessage : MvxMessage
    {
        /// <summary>
        /// New user information
        /// </summary>
        public User User { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public UserAuthenticatedMessage(object sender, User user) : base(sender)
        {
            this.User = user;
        }
    }
}
