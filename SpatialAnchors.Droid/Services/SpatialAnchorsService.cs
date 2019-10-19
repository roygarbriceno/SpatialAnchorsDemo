using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.SpatialAnchors;
using Java.Util.Concurrent;
using SpatialAnchors.Core.Interfaces;
using Google.AR.Core;
using Android.App;
using Google.AR.Sceneform;
using SpatialAnchors.Core;
using Android.Content;
using SpatialAnchors.Droid.Models;
using System.Collections.Concurrent;
using Google.AR.Sceneform.Rendering;
using Java.Util;
using Google.AR.Sceneform.UX;

namespace SpatialAnchors.Droid.Services
{
    /// <summary>
    /// Spatial Anchors android implementation
    /// </summary>
    public class SpatialAnchorsService : ISpatialAnchorsService
    {
        private ArFragment arFragment;
        private ModelRenderable modelRenderable;                
        private Context context;
        private CloudSpatialAnchorSession spatialAnchorsSession;
        private TrackingState lastTrackingState = TrackingState.Stopped;
        private TrackingFailureReason lastTrackingFailureReason = TrackingFailureReason.None;
        private readonly object progressLock = new object();
        private readonly ConcurrentDictionary<string, AnchorModel> anchorVisuals = new ConcurrentDictionary<string, AnchorModel>();
                

        /// </inheritdoc>
        public SpatialAnchorsMode Mode { get; protected set; }


        /// </inheritdoc>
        public SpatialAnchorStatus Status { get; protected set; }


        /// </inheritdoc>
        public EventHandler<SpatialAnchors.Models.Anchor> SaveAnchor { get; set; }


        /// </inheritdoc>
        public EventHandler<string> ShowMessage { get; set; }


        /// </inheritdoc>
        public void StartSession(object context, object arScene)
        {            
            this.context = context as Context; 
            this.arFragment = arScene as ArFragment;

            if (this.spatialAnchorsSession == null)
            {
                CloudServices.Initialize(this.context);
            }

            this.spatialAnchorsSession = new CloudSpatialAnchorSession();
            this.spatialAnchorsSession.Configuration.AccountKey = Constants.SpatialAnchorsAccountKey;
            this.spatialAnchorsSession.Configuration.AccountId = Constants.SpatialAnchorsAccountId;
            this.spatialAnchorsSession.Session = this.arFragment.ArSceneView.Session;
            this.spatialAnchorsSession.AnchorLocated += this.OnAnchorLocated;
            this.spatialAnchorsSession.LocateAnchorsCompleted += this.OnLocateAnchorsCompleted;
            this.spatialAnchorsSession.SessionUpdated += this.SessionUpdated;
            this.spatialAnchorsSession.TokenRequired += SpatialAnchorsSession_TokenRequired;
            this.spatialAnchorsSession.Error += (sender, e) =>
            {
                SessionErrorEvent eventArgs = e?.Args;
                if (eventArgs == null) return;
                var message = $"{eventArgs.ErrorCode}: {eventArgs.ErrorMessage}";
                ShowMessage(this, message);
            };
            

            this.spatialAnchorsSession.Start();           
            this.Status  = SpatialAnchorStatus.Iddle;
            this.arFragment.TapArPlane += OnTapArPlane;                        
        }



        private async void SpatialAnchorsSession_TokenRequired(object sender, TokenRequiredEventArgs e)
        {
            var token = await this.spatialAnchorsSession.GetAccessTokenWithAccountKeyAsync(Constants.SpatialAnchorsAccountKey).GetAsync();     
        }


        /// <summary>
        /// 
        /// </summary>        
        private void SessionUpdated(object sender, SessionUpdatedEventArgs e)
        {
           if (this.Mode == SpatialAnchorsMode.AddAnchors && this.Status == SpatialAnchorStatus.Scanning)
            {
                float progress = e.Args.Status.RecommendedForCreateProgress;
                var enoughDataForSaving = progress >= 1.0;
                lock (this.progressLock)
                {

                    if (this.Status == SpatialAnchorStatus.Scanning && !enoughDataForSaving)
                    {                     
                        return;
                    }
                    if (enoughDataForSaving)
                    {
                        if (this.Status == SpatialAnchorStatus.Saving) return;
                        if (this.anchorVisuals.TryGetValue(string.Empty, out AnchorModel model))
                        {
                            try
                            {
                                this.Status = SpatialAnchorStatus.Saving;
                                var cloudAnchor = new CloudSpatialAnchor
                                {
                                    LocalAnchor = model.LocalAnchor.Anchor
                                };
                                model.CloudAnchor = cloudAnchor;
                                var now = new Date();
                                var calendar = Calendar.Instance;
                                calendar.Time = now;
                                calendar.Add(CalendarField.Date, 7);
                                var oneWeekFromNow = calendar.Time;
                                cloudAnchor.Expiration = oneWeekFromNow;
                                Task.Run(async () =>
                                {
                                    try
                                    {                                      
                                        var result = await this.spatialAnchorsSession.CreateAnchorAsync(cloudAnchor).GetAsync();
                                        var anchorId = cloudAnchor.Identifier;
                                        this.anchorVisuals[anchorId] = model;
                                        this.anchorVisuals.TryRemove(string.Empty, out _);
                                        SaveAnchor(this, new SpatialAnchors.Models.Anchor { AnchorId = cloudAnchor.Identifier });                                        
                                        this.Status = SpatialAnchorStatus.Iddle;
                                    }
                                    catch (Exception ex)
                                    {
                                        ShowMessage(this, "ErrorSavingAnchor");
                                    }
                                });
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
        }


        /// </inheritdoc>
        public void StopSession()
        {            
            if (this.Status != SpatialAnchorStatus.NotStarted)
            {
                StopLocatingAnchors();
                this.spatialAnchorsSession.Stop();
                this.Status = SpatialAnchorStatus.NotStarted;
            }
        }


        /// </inheritdoc>
        public void ProcessFrame(object frame)
        {
            var arFrame = frame as Frame;
            if (arFrame.Camera.TrackingState != this.lastTrackingState
                || arFrame.Camera.TrackingFailureReason != this.lastTrackingFailureReason)
            {
                this.lastTrackingState = arFrame.Camera.TrackingState;
                this.lastTrackingFailureReason = arFrame.Camera.TrackingFailureReason;
            }
            Task.Run(() => this.spatialAnchorsSession.ProcessFrame(arFrame));
        }

        
        /// <summary>
        /// Called when it finish to locating anchors
        /// </summary>       
        private void OnLocateAnchorsCompleted(object sender, LocateAnchorsCompletedEventArgs e)
        {
            StopLocatingAnchors();          
        }


        /// </inheritdoc>  
        public void PlaceModel(object transform)
        {

        }


        /// </inheritdoc>  
        public void StartLocatingAnchors(string[] anchors)
        {
            this.Mode = SpatialAnchorsMode.SearchAnchors;
            StopLocatingAnchors();            
            var criteria = new AnchorLocateCriteria();
            criteria.SetIdentifiers(anchors);
            this.spatialAnchorsSession.CreateWatcher(criteria);
        }



        /// <summary>
        /// Stops searching for anchors, only one watcher is active
        /// </summary>
        private void StopLocatingAnchors()
        {
            var watcher = this.spatialAnchorsSession.ActiveWatchers.FirstOrDefault();
            watcher?.Stop();
            watcher?.Dispose();
        }


        /// <summary>
        /// Called when an anchor is found
        /// </summary>        
        private void OnAnchorLocated(object sender, AnchorLocatedEventArgs e)
        {
            var status = e.Args.Status;
            if (status == LocateAnchorStatus.AlreadyTracked)
            {
                // Nothing to do since we've already rendered any anchors we've located.
            }
            else if (status == LocateAnchorStatus.Located)
            {                
                var activity = this.context as Activity;
                activity?.RunOnUiThread(() =>
                {
                    var cloudAnchor = e.Args.Anchor;
                    var model = CreateModel(new AnchorNode(cloudAnchor.LocalAnchor), e.Args.Anchor);
                    this.anchorVisuals[cloudAnchor.Identifier] = model;
                });
            }
        }


        /// <summary>
        /// Adds a model whe the user taps on the plane
        /// </summary>        
        private void OnTapArPlane(object sender, BaseArFragment.TapArPlaneEventArgs e)
        {
            if (modelRenderable == null) return;
            if (this.Mode == SpatialAnchorsMode.AddAnchors &&
                this.Status == SpatialAnchorStatus.Iddle)
            {
                var model = CreateModel(new AnchorNode(e.HitResult.CreateAnchor()), null);
                this.anchorVisuals[string.Empty] = model;                
                this.Status = SpatialAnchorStatus.Scanning; 
            }
        }


        /// <summary>
        /// Creates 3D model with the specified anchors
        /// </summary>        
        private AnchorModel CreateModel(AnchorNode localAnchor, CloudSpatialAnchor cloudAnchor)
        {
            var anchorModel = new AnchorModel
            {
                LocalAnchor = localAnchor,
                CloudAnchor = cloudAnchor
            };
            localAnchor.SetParent(arFragment.ArSceneView.Scene);
            var model = new TransformableNode(arFragment.TransformationSystem);
            model.SetParent(localAnchor);
            model.Renderable = this.modelRenderable;
            model.Select();
            return anchorModel;
        }


        /// <summary>
        /// Loads the 3D models to use        
        /// </summary>
        public void LoadModels()
        {
            ModelRenderable.InvokeBuilder().SetSource(this.context, Resource.Raw.andy).Build(renderable =>
            {
                modelRenderable = renderable;
            });
        }
    }
}