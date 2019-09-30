//namespace SpatialAnchors.Core.Services
//{
//    using System.Threading.Tasks;

//    /// <summary>
//    /// Basse Spatcial Anchors service
//    /// </summary>
//    public abstract class BaseSpatiaAnchorsService
//    {
//        protected string spatialAnchorsAccountId = "fe723cf9-aaed-455f-bd36-322a14657249";
//        protected string spatialAnchorsAccountKey = "o5dc40N0mFQ1YDFqhddcTf3WijFf9X4vylVmo+Nu5E0=";

//        /// <summary>
//        /// Initializes the Spatial Anchors AR session
//        /// </summary>
//        /// <param name="session"></param>
//        public abstract void Initialize(object session);


//        /// <summary>
//        /// Starts the AR session
//        /// </summary>
//        public abstract void StartSession();


//        /// <summary>
//        /// Stops the AR sessions
//        /// </summary>
//        public abstract void StopSession();


//        /// <summary>
//        /// Start searching for anchors
//        /// </summary>
//        public abstract void StartSearching();


//        /// <summary>
//        /// Stop searching for anchors
//        /// </summary>
//        public abstract void StopSearching();


//        /// <summary>
//        /// Updates a frame
//        /// </summary>        
//        public abstract void Update(object frame);


//        /// <summary>
//        /// Creates a new spatial anchors
//        /// </summary>        
//        public abstract Task<object> CreateAnchorAsync(object platformAnchor);
//    }
//}
