//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Azure.SpatialAnchors;
//using SpatialAnchors.Core.Services;
//using Java.Util.Concurrent;
//using SpatialAnchors.Core.Interfaces;
//using Google.AR.Core;

//namespace SpatialAnchors.Droid.Services
//{
//    /// <summary>
//    /// Spatial Anchors android implementation
//    /// </summary>
//    public class SpatialAnchorsService : BaseSpatiaAnchorsService, ISpatialAnchorsService
//    {
//        private bool sessionStarted = false;
//        private CloudSpatialAnchorSession spatialAnchorsSession;
//        private TrackingState lastTrackingState = TrackingState.Stopped;
//        private TrackingFailureReason lastTrackingFailureReason = TrackingFailureReason.None;


//        public override void Initialize(object session)
//        {
//            CloudServices.Initialize(Android.App.Application.Context);

//            this.spatialAnchorsSession = new CloudSpatialAnchorSession();
//            this.spatialAnchorsSession.Configuration.AccountKey = this.spatialAnchorsAccountKey;
//            this.spatialAnchorsSession.Configuration.AccountId = this.spatialAnchorsAccountId;

//            this.spatialAnchorsSession.Session = (Google.AR.Core.Session)session;
//            this.spatialAnchorsSession.LogDebug += this.LogDebug;
//            this.spatialAnchorsSession.Error += this.Error;
//            this.spatialAnchorsSession.AnchorLocated += this.AnchorLocated;
//            this.spatialAnchorsSession.LocateAnchorsCompleted += this.LocateAnchorsCompleted;
//            this.spatialAnchorsSession.SessionUpdated += this.SessionUpdated;
//        }


//        /// <summary>
//        /// Starts the AR session
//        /// </summary>
//        public override void StartSession()
//        {
//            this.spatialAnchorsSession.Start();
//            this.sessionStarted = true;
//        }


//        /// <summary>
//        /// Stops the AR sessions
//        /// </summary>
//        public override void StopSession()
//        {
//            StopSearching();
//            if (this.sessionStarted)
//            {
//                this.spatialAnchorsSession.Stop();
//                this.sessionStarted = false;
//            }
//        }


//        /// <summary>
//        /// Start searching for anchors
//        /// </summary>
//        public override void StartSearching()
//        {
//            StopSearching();
//            //return this.spatialAnchorsSession.CreateWatcher(locateCriteria);
//        }


//        /// </inheritdoc>
//        public override void StopSearching()
//        {
//            var watcher = this.spatialAnchorsSession.ActiveWatchers.FirstOrDefault();            
//            watcher?.Stop();
//            watcher?.Dispose();
//        }


//        /// </inheritdoc>
//        public override void Update(object frame)
//        {
//            var arFrame = frame as Google.AR.Core.Frame;
//            if (arFrame.Camera.TrackingState != this.lastTrackingState
//                || arFrame.Camera.TrackingFailureReason != this.lastTrackingFailureReason)
//            {
//                this.lastTrackingState = arFrame.Camera.TrackingState;
//                this.lastTrackingFailureReason = arFrame.Camera.TrackingFailureReason;                
//            }
//            Task.Run(() => this.spatialAnchorsSession.ProcessFrame(arFrame));
//        }


//        /// </inheritdoc>  
//        private void LogDebug(object sender, LogDebugEventArgs e)
//        {
//            if (string.IsNullOrWhiteSpace(e.Args.Message))
//            {
//                return;
//            }
//            //Debug.WriteLine(e.Args.Message);
//            //this.OnLogDebug?.Invoke(sender, e);
//        }


//        /// </inheritdoc>
//        private void SessionUpdated(object sender, SessionUpdatedEventArgs e)
//        {
//            var createScanProgress = Math.Min(e.Args.Status.RecommendedForCreateProgress, 1);

//            //Debug.WriteLine($"Create scan progress: {createScanProgress:0%}");
//            //this.CreateScanningProgressValue = createScanProgress;
//            //this.OnSessionUpdated?.Invoke(sender, e.Args);
//        }


//        /// </inheritdoc>
//        private void Error(object sender, SessionErrorEventArgs e)
//        {
//            SessionErrorEvent eventArgs = e?.Args;
//            /*if (eventArgs == null)
//            {
//                Debug.WriteLine("Azure Spatial Anchors reported an unspecified error.");
//                return;
//            }
//            string message = $"{eventArgs.ErrorCode}: {eventArgs.ErrorMessage}";
//            Debug.WriteLine(message);
//            this.OnSessionError?.Invoke(sender, eventArgs);*/
//        }


//        /// <summary>
//        /// Called when finishing locating an specific anchor
//        /// </summary>        
//        private void AnchorLocated(object sender, AnchorLocatedEventArgs e)
//        {
//            //this.OnAnchorLocated?.Invoke(sender, e.Args);
//        }


//        /// <summary>
//        /// Called when finished locating an anchors
//        /// </summary>
//        private void LocateAnchorsCompleted(object sender, LocateAnchorsCompletedEventArgs e)
//        {
//            //this.OnLocateAnchorsCompleted?.Invoke(sender, e.Args);
//        }


//        /// <summary>
//        /// Creates a new anchor
//        /// </summary>        
//        public override async Task<object> CreateAnchorAsync(object platformAnchor)
//        {
//            var newCloudAnchor = platformAnchor as CloudSpatialAnchor;
//            if (newCloudAnchor == null)
//            {
//                throw new ArgumentNullException(nameof(newCloudAnchor));
//            }
//            if (newCloudAnchor.LocalAnchor == null || !string.IsNullOrEmpty(newCloudAnchor.Identifier))
//            {
//                throw new ArgumentException("The specified cloud anchor cannot be saved.", nameof(newCloudAnchor));
//            }
//            //if (!this.CanCreateAnchor)
//            //{
//            //    throw new ArgumentException("Not ready to create. Need more data.");
//            //}
//            try
//            {
//                await this.spatialAnchorsSession.CreateAnchorAsync(newCloudAnchor).GetAsync();
//            }
//            catch (Exception ex)
//            {
//               // Debug.WriteLine(ex.Message);
//            }
//            return newCloudAnchor;
//        }
//    }
//}