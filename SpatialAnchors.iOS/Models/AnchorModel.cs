namespace SpatialAnchors.IOS.Models
{
    using ARKit;
    using Microsoft.Azure.SpatialAnchors;
    using SceneKit;


    /// <summary>
    /// AR Anchor for iOS
    /// for holding both the local anchor and related cloud anchor
    /// </summary>
    public class AnchorModel
    {
        /// <summary>
        /// Scene kit node
        /// </summary>
        public SCNNode Node { get; set; }
 

        /// <summary>
        /// Cloud anchor (Azure)
        /// </summary>
        public CloudSpatialAnchor CloudAnchor { get; set; }
        

        /// <summary>
        /// Local anchor
        /// </summary>
        public ARAnchor LocalAnchor { get; set; }
    }
}