using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.SpatialAnchors;
using SpatialAnchors.Core.Interfaces;
using SpatialAnchors.Core;
using System.Collections.Concurrent;
using SpatialAnchors.IOS.Models;
using ARKit;
using SceneKit;
using OpenTK;



namespace SpatialAnchors.iOS.Services
{
    /// <summary>
    /// Spatial Anchors for ios
    /// </summary>
    public class SpatialAnchorsService : ISpatialAnchorsService
    {
        private ARSCNView sceneView;
        private CloudSpatialAnchorSession spatialAnchorsSession;
        private bool enoughDataForSaving;
        private readonly ConcurrentDictionary<string, AnchorModel> anchorVisuals = new ConcurrentDictionary<string, AnchorModel>();
        private ARAnchor localAnchor;               // Local temp anchor when creating them
        private CloudSpatialAnchor cloudAnchor;     // Local temp clound anchor when saving them

        /// </inheritdoc>
        public SpatialAnchorsMode Mode { get; protected set; }


        /// </inheritdoc>
        public SpatialAnchorStatus Status { get; protected set; }


        /// </inheritdoc>
        public EventHandler<Models.Anchor> SaveAnchor { get; set; }


        /// </inheritdoc>
        public EventHandler<string> ShowMessage { get; set; }


        /// </inheritdoc>
        public void StartSession(object context, object arScene)
        {          
            this.sceneView = arScene as ARSCNView;
            this.spatialAnchorsSession = new CloudSpatialAnchorSession();
            
            this.spatialAnchorsSession.Configuration.AccountKey = Constants.SpatialAnchorsAccountKey;
            this.spatialAnchorsSession.Configuration.AccountId = Constants.SpatialAnchorsAccountId;
            this.spatialAnchorsSession.Session = this.sceneView.Session;
            this.spatialAnchorsSession.AnchorLocated += this.OnAnchorLocated;
            this.spatialAnchorsSession.LocateAnchorsCompleted += this.OnLocateAnchorsCompleted;
            this.spatialAnchorsSession.SessionUpdated += this.SessionUpdated;
            this.spatialAnchorsSession.Error += (sender, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.ErrorMessage))
                {
                    ShowMessage(this, e.ErrorMessage);
                }
            };

            this.spatialAnchorsSession.Start();           
            this.Status  = SpatialAnchorStatus.Iddle;
            this.enoughDataForSaving = false;
        }



        /// <summary>
        /// 
        /// </summary>        
        private void SessionUpdated(object sender, SessionUpdatedEventArgs e)
        {
            if (this.Mode == SpatialAnchorsMode.AddAnchors && this.Status == SpatialAnchorStatus.Scanning)
            {
                SessionStatus status = e.Status;
                this.enoughDataForSaving = status.RecommendedForCreateProgress >= 1.0;

                if (this.enoughDataForSaving)
                {
                    // on IOS saving an anchor happern during the process frame
                   
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
                foreach (var visual in this.anchorVisuals.Values)
                {
                    if (visual.Node != null)
                    {
                        visual.Node.RemoveFromParentNode();
                    }
                }
                this.anchorVisuals.Clear();
            }
        }


        /// </inheritdoc>
        public void ProcessFrame(object frame)
        {
            var arFrame = frame as ARFrame;
            if (frame == null) return;
            if (this.spatialAnchorsSession == null) return;

            this.spatialAnchorsSession.ProcessFrame(arFrame);

            if (this.enoughDataForSaving && this.Mode == SpatialAnchorsMode.AddAnchors)
            {
                if (this.Status == SpatialAnchorStatus.Saving) return;
                if (this.anchorVisuals.TryGetValue(string.Empty, out AnchorModel model))
                {
                    this.Status = SpatialAnchorStatus.Saving;
                    this.cloudAnchor = new CloudSpatialAnchor
                    {
                        LocalAnchor = this.localAnchor
                    };

                    // In this sample app we delete the cloud anchor explicitly, but you can also set it to expire automatically
                    DateTime now = DateTime.Today;
                    DateTimeOffset oneWeekFromNow = now.AddDays(7);
                    this.cloudAnchor.Expiration = oneWeekFromNow;

                    Task.Run(async () =>
                    {
                        try
                        {
                            await this.spatialAnchorsSession.CreateAnchorAsync(cloudAnchor);
                            var anchorId = cloudAnchor.Identifier;
                            this.anchorVisuals[anchorId] = model;
                            this.anchorVisuals.TryRemove(string.Empty, out _);
                            SaveAnchor(this, new Models.Anchor { AnchorId = cloudAnchor.Identifier });
                           
                        }
                        catch (Exception ex)
                        {
                            ShowMessage(this, "ErrorSavingAnchor");
                        }
                        finally
                        {
                            this.Status = SpatialAnchorStatus.Iddle;
                        }
                    });
                }
            }
        }

        
        /// <summary>
        /// Called when it finish to locating anchors
        /// </summary>       
        private void OnLocateAnchorsCompleted(object sender, LocateAnchorsCompletedEventArgs e)
        {
            StopLocatingAnchors();          
        }


        /// </inheritdoc>  
        public void StartLocatingAnchors(string[] anchors)
        {
            this.Mode = SpatialAnchorsMode.SearchAnchors;
            StopLocatingAnchors();
            var criteria = new AnchorLocateCriteria
            {
                Identifiers = anchors
            };
            this.spatialAnchorsSession.CreateWatcher(criteria);
        }


        /// <summary>
        /// Stops searching for anchors, only one watcher is active
        /// </summary>
        private void StopLocatingAnchors()
        {
            var watcher = this.spatialAnchorsSession.GetActiveWatchers().FirstOrDefault();
            watcher?.Stop();
        }


        /// <summary>
        /// Called when an anchor is found
        /// </summary>        
        private void OnAnchorLocated(object sender, AnchorLocatedEventArgs e)
        {
            if (e.Status == LocateAnchorStatus.AlreadyTracked)
            {
                // Nothing to do since we've already rendered any anchors we've located.
            }
            else if (e.Status == LocateAnchorStatus.Located)
            {
                var anchor = e.Anchor;                
                var model = new AnchorModel
                {
                    CloudAnchor = anchor,                    
                    LocalAnchor = anchor.LocalAnchor
                };
                this.anchorVisuals[anchor.Identifier] = model;
                this.sceneView.Session.AddAnchor(anchor.LocalAnchor);

                var modelNode = LoadMOdel();                
                modelNode.Position = model.LocalAnchor.Transform.ToPosition();               
                this.sceneView.Scene.RootNode.AddChildNode(modelNode);            
            }
        }


        /// <summary>
        /// Places a model on a point
        /// </summary>   
        public void PlaceModel(object transform)
        {      
            if (this.Mode == SpatialAnchorsMode.AddAnchors &&
                this.Status == SpatialAnchorStatus.Iddle)
            {
                var matrix = (NMatrix4)transform;
                this.localAnchor = new ARAnchor(matrix);

                var model = new AnchorModel
                {
                    CloudAnchor = null,
                    LocalAnchor = this.localAnchor
                };
                this.anchorVisuals[string.Empty] = model;

                var modelNode = LoadMOdel();
                modelNode.Position = model.LocalAnchor.Transform.ToPosition();
                this.sceneView.Scene.RootNode.AddChildNode(modelNode);
                this.Status = SpatialAnchorStatus.Scanning;
            }
        }

        

        /// <summary>
        /// Load the Andy Android Model
        /// </summary>        
        private SCNNode LoadMOdel()
        {
            try
            {
                var modelName = "art.scnassets/andy.usdz";                    
                var scene = SCNScene.FromFile(modelName);
                var modelNode = scene.RootNode.ChildNodes[0];
             modelNode.Scale = new SCNVector3(0.8f, 0.8f, 0.8f);
                return modelNode;
            }
            catch (Exception ex)
            {
                ShowMessage(this, "Error loading model");
            }
            return null;
        }



        /// <summary>
        /// Loads the 3D models to use        
        /// </summary>
        public void LoadModels()
        {            
        }
    }

    public static class NMatrix4Extensions
    {
        /// <summary>
        /// Converts a transform to a position.
        /// </summary>
        public static SCNVector3 ToPosition(this NMatrix4 transform)
        {
            return new SCNVector3(transform.M14, transform.M24, transform.M34);
        }
    }
}