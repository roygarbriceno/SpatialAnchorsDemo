//namespace SpatialAnchors.Core.Interfaces
//{
//    using System.Threading.Tasks;

//    /// <summary>
//    /// Common interface for the spatial anchor service
//    /// </summary>
//    public interface ISpatialAnchorsService
//    {
//        /// <summary>
//        /// Initializes the Spatial Anchors AR session
//        /// </summary>
//        /// <param name="session"></param>
//        void Initialize(object session);


//        /// <summary>
//        /// Starts the AR session
//        /// </summary>
//        void StartSession();


//        /// <summary>
//        /// Stops the AR sessions
//        /// </summary>
//        void StopSession();


//        /// <summary>
//        /// Start searching for anchors
//        /// </summary>
//        void StartSearching();


//        /// <summary>
//        /// Stop searching for anchors
//        /// </summary>
//        void StopSearching();


//        /// <summary>
//        /// Updates a frame
//        /// </summary>        
//        void Update(object frame);


//        /// <summary>
//        /// Creates a new spatial anchors
//        /// </summary>        
//        Task<object> CreateAnchorAsync(object platformAnchor);
//    }
//}
