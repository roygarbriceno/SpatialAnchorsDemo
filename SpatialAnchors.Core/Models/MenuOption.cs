namespace SpatialAnchors.Core.Models
{
    using System.Windows.Input;

    /// <summary>
    /// Side menu option
    /// </summary>
    public class MenuOption
    {
        /// <summary>
        /// Gets or Set the MenuOption Description
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Gets or Set the MenuOption NavigationCommand
        /// </summary>
        public ICommand Command { get; set; }


        /// <summary>
        /// Gets or Set the MenuOption Icon
        /// </summary>
        public string Icon { get; set; }
    }
}
