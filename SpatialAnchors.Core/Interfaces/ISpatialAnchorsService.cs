namespace SpatialAnchors.Core.Interfaces
{
    using System;
    using SpatialAnchors.Models;


    /// <summary>
    /// Common interface for the AR service with spatial anchors support
    /// </summary>
    public interface ISpatialAnchorsService
    {
        /// <summary>
        /// Current mode of the ar sersion
        /// </summary>
        SpatialAnchorsMode Mode { get; }


        /// <summary>
        /// Current status of spatial anchor session
        /// </summary>
        SpatialAnchorStatus Status { get; }


        /// <summary>
        /// Called to save an anchor
        /// </summary>
        EventHandler<Anchor> SaveAnchor { get; set; }


        /// <summary>
        /// Called to show a message to the user
        /// </summary>
        EventHandler<string> ShowMessage { get; set; }


        /// <summary>
        /// Initializes the Spatial Anchors AR session
        /// </summary>        
        void StartSession(object context, object scene);


        /// <summary>
        /// Starts locating a collection of anchors
        /// </summary>        
        void StartLocatingAnchors(string[] anchors);

      
        /// <summary>
        /// Stops the AR sessions
        /// </summary>
        void StopSession();

        /// <summary>
        /// Loasd the 3D models to use
        /// </summary>
        void LoadModels();

       
        /// <summary>
        /// Updates a frame
        /// </summary>        
        void ProcessFrame(object frame);

    }
}
