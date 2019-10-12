namespace SpatialAnchors.Droid.Models
{
    using Google.AR.Sceneform;
    using Microsoft.Azure.SpatialAnchors;

    /// <summary>
    /// AR Anchor for Android
    /// for holding both the local anchor and related cloud anchor
    /// </summary>
    public class AnchorModel
    {
        /// <summary>
        /// Local AR anchor
        /// </summary>
        public AnchorNode LocalAnchor { get; set; }

        /// <summary>
        /// Anchor saved on the spatial anchor account
        /// </summary>
        public CloudSpatialAnchor CloudAnchor { get; set; }
    }
}