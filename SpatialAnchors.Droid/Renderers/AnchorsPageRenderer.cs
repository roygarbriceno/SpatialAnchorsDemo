using Android.App;
using Android.Content;
using Android.Views;
using Google.AR.Sceneform;
using Google.AR.Sceneform.Rendering;
using Google.AR.Sceneform.UX;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Google.AR.Core;
using static Google.AR.Sceneform.Scene;
using System;
using Android.Support.Design.Widget;
using MvvmCross.Plugin.JsonLocalization;
using MvvmCross;
using SpatialAnchors.Core.ViewModels;
using SpatialAnchors.Core.Services;
using SpatialAnchors.Core.Pages;
using SpatialAnchors.Droid.Renderers;
using SpatialAnchors.Droid.Fragments;
using Java.Util.Functions;
using Java.Lang;
using SpatialAnchors.Core.Arguments;
using SpatialAnchors.Core.Interfaces;
using SpatialAnchors.Droid.Models;
using Microsoft.Azure.SpatialAnchors;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Java.Util;
using Java.Util.Concurrent;
using System.Linq;

[assembly: ExportRenderer(typeof(AnchorsPage), typeof(AnchorsPageRenderer))]
namespace SpatialAnchors.Droid.Renderers
{
    /// <summary>
    /// Page Renderer for our AR session
    /// </summary>
    public class AnchorsPageRenderer : PageRenderer//, IOnUpdateListener//, IConsumer
    {
        private readonly Context context;
        private ArFragment arFragment;
        private ModelRenderable modelRenderable;
        private Android.Views.View view;
        private AnchorsViewModel viewModel;
        protected Config config;

        private ArSceneView sceneView;
        protected bool modelAdded = false;
        private IMvxTextProviderBuilder textProviderBuilder;
        //private ISpatialAnchorsService spatialAnchorsService;

        private bool sessionStarted = false;
        private CloudSpatialAnchorSession spatialAnchorsSession;
        private TrackingState lastTrackingState = TrackingState.Stopped;
        private TrackingFailureReason lastTrackingFailureReason = TrackingFailureReason.None;
        private readonly object progressLock = new object();
        private readonly ConcurrentDictionary<string, AnchorModel> anchorVisuals = new ConcurrentDictionary<string, AnchorModel>();

        protected string spatialAnchorsAccountId = "fe723cf9-aaed-455f-bd36-322a14657249";
        protected string spatialAnchorsAccountKey = "o5dc40N0mFQ1YDFqhddcTf3WijFf9X4vylVmo+Nu5E0=";
        private bool savingAnchor = false;

        /// <summary>
        /// Constructor
        /// </summary>        
        public AnchorsPageRenderer(Context context) : base(context)
        {
            this.AutoPackage = false;
            this.context = context;
        }


        /// <summary>
        /// Gets a reference to the ViewModel and the ArFragment
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Page> e)
        {
            try
            {
                base.OnElementChanged(e);
                var activity = this.Context as Activity;

                this.viewModel = this.Element.BindingContext as AnchorsViewModel;

                // Gets the view
                this.view = activity.LayoutInflater.Inflate(Resource.Layout.AnchorsLayout, this, false);
                AddView(this.view);

                this.textProviderBuilder = Mvx.IoCProvider.GetSingleton<IMvxTextProviderBuilder>();
                
                // Setups the AR fragment
                this.arFragment = activity.GetFragmentManager().FindFragmentById(Resource.Id.anchors_fragment) as AnchorsFragment;
                if (this.arFragment != null)
                {
                    this.sceneView = this.arFragment.ArSceneView;
                    this.sceneView.Scene.Update += (_, args) =>
                    {
                        // Request frame to be processed by the CloudSpatialAnchorSession
                        ProcessFrame(this.sceneView.ArFrame);                        
                    };

                    //this.arFragment.ArSceneView.Scene.AddOnUpdateListener(this);
                    ModelRenderable.InvokeBuilder().SetSource(this.context, Resource.Raw.andy).Build(renderable =>
                    {
                        modelRenderable = renderable;
                    });

                    arFragment.TapArPlane += OnTapArPlane;

                    // Starts the session
                    StartSpatialAnchorsSession();

                    if (this.viewModel.Parameters.Mode == Core.SpatialAnchorsMode.Searching)
                    {
                        StartLocatingAnchors();
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Show a message to the user
                ShowMessage("ArNotAvailable");
            }
        }
    


        /// Starts the AR session
        /// </summary>
        public void StartSpatialAnchorsSession()
        {
            CloudServices.Initialize(Android.App.Application.Context);

            this.spatialAnchorsSession = new CloudSpatialAnchorSession();
             this.spatialAnchorsSession.Configuration.AccountKey = this.spatialAnchorsAccountKey;
            this.spatialAnchorsSession.Configuration.AccountId = this.spatialAnchorsAccountId;

            this.spatialAnchorsSession.Session = this.sceneView.Session;
            this.spatialAnchorsSession.LogDebug += this.LogDebug;
            this.spatialAnchorsSession.Error += this.Error;
            this.spatialAnchorsSession.AnchorLocated += this.OnAnchorLocated;
            this.spatialAnchorsSession.LocateAnchorsCompleted += this.OnLocateAnchorsCompleted;
            this.spatialAnchorsSession.SessionUpdated += this.SessionUpdated;

            this.spatialAnchorsSession.Start();
            this.sessionStarted = true;
        }


        /// <summary>
        /// 
        /// </summary>        
        private void SessionUpdated(object sender, SessionUpdatedEventArgs e)
        {
            //var createScanProgress = System.Math.Min(e.Args.Status.RecommendedForCreateProgress, 1);            
            if (this.viewModel.Parameters.Mode == Core.SpatialAnchorsMode.Adding)
            {
                float progress = e.Args.Status.RecommendedForCreateProgress;
                var enoughDataForSaving = progress >= 1.0;
                lock (this.progressLock)
                {

                    if (this.savingAnchor)
                    {
                        ShowMessage($"Scan progress is {System.Math.Min(1.0f, progress):0%}");
                    }

                    if (enoughDataForSaving)
                    {
                        if (this.anchorVisuals.TryGetValue(string.Empty, out AnchorModel model))
                        {
                            try
                            {
                                var cloudAnchor = new CloudSpatialAnchor
                                {
                                    LocalAnchor = model.AnchorNode.Anchor
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
                                    var result = await this.spatialAnchorsSession.CreateAnchorAsync(cloudAnchor).GetAsync();
                                    var anchorId = cloudAnchor.Identifier;
                                    this.anchorVisuals[anchorId] = model;
                                    this.anchorVisuals.TryRemove(string.Empty, out _);

                                    ShowMessage("Anchor saved");
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


        /// <summary>
        /// Stops the AR sessions
        /// </summary>
        public void StopSession()
        {
            //StopSearching();
            if (this.sessionStarted)
            {
                this.spatialAnchorsSession.Stop();
                this.sessionStarted = false;
            }
        }



        /// </inheritdoc>
        public void ProcessFrame(Google.AR.Core.Frame frame)
        {
            var arFrame = frame as Google.AR.Core.Frame;
            if (arFrame.Camera.TrackingState != this.lastTrackingState
                || arFrame.Camera.TrackingFailureReason != this.lastTrackingFailureReason)
            {
                this.lastTrackingState = arFrame.Camera.TrackingState;
                this.lastTrackingFailureReason = arFrame.Camera.TrackingFailureReason;
            }
            Task.Run(() => this.spatialAnchorsSession.ProcessFrame(arFrame));
        }


        /// <summary>
        /// Adds a model whe the user taps on the plane
        /// </summary>        
        private void OnTapArPlane(object sender, BaseArFragment.TapArPlaneEventArgs e)
        {
            /*

            // Create the Anchor.
            var anchor = e.HitResult.CreateAnchor();
            var anchorNode = new AnchorNode(anchor);
            anchorNode.SetParent(arFragment.ArSceneView.Scene);

            // Create the transformable andy and add it to the anchor.
            var andy = new TransformableNode(arFragment.TransformationSystem);
            andy.SetParent(anchorNode);
            andy.Renderable = modelRenderable;
            andy.Select();*/

            if (modelRenderable == null) return;
            if (this.viewModel.Parameters.Mode == Core.SpatialAnchorsMode.Adding)
            {
                var anchorModel = new AnchorModel
                {
                    AnchorNode = new AnchorNode(e.HitResult.CreateAnchor()),
                };
                anchorModel.AddToScene(modelRenderable, arFragment);

                // Saves the the anchor without a key
                // The key will be set when the anchor is saved
                this.anchorVisuals[string.Empty] = anchorModel;

                this.savingAnchor = true;
                /*AnchorVisual visual = new AnchorVisual(hitResult.CreateAnchor());
                visual.SetColor(readyColorMaterial);
                visual.AddToScene(this.arFragment);
                this.anchorVisuals[string.Empty] = visual;

                this.RunOnUiThread(() =>
                {
                    this.scanProgressText.Visibility = ViewStates.Visible;
                    if (this.enoughDataForSaving)
                    {
                        this.statusText.Text = "Ready to save";
                        this.actionButton.Text = "Save cloud anchor";
                        this.actionButton.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        this.statusText.Text = "Move around the anchor";
                    }
                });

                this.currentDemoStep = DemoStep.SaveAnchor;

                return visual.LocalAnchor;*/
            }
        }


        /// <summary>
        /// Fix the layout measures to fill the whole view
        /// </summary>        
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);
            this.view.Measure(msw, msh);
            this.view.Layout(0, 0, r - l, b - t);
        }


        /// <summary>
        /// Remoev the AR fragment when the pages closes otherwise will throw an error when returning
        /// </summary>
        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            var activity = this.Context as Activity;
            activity.GetFragmentManager().BeginTransaction().Remove(this.arFragment).Commit();
        }


        ///// <summary>
        ///// Checks the update frame to track the images
        ///// </summary>        
        //public void OnUpdate(FrameTime frameTime)
        //{

        //    try
        //    {
        //        //if (this.arFragment != null)
        //        //{
        //        //    var frame = this.arFragment.ArSceneView.ArFrame;
        //        //    var updatedAugmentedImages = frame.GetUpdatedTrackables(Java.Lang.Class.FromType(typeof(AugmentedImage)));
        //        //    foreach (var image in updatedAugmentedImages)
        //        //    {
        //        //        // Adds a model over the picture
        //        //        var augmentedImage = (AugmentedImage)image;
        //        //        if (augmentedImage.TrackingState == TrackingState.Tracking && !this.modelAdded)
        //        //        {
        //        //            var pose = augmentedImage.CenterPose;
        //        //            var anchor = augmentedImage.CreateAnchor(augmentedImage.CenterPose);
        //        //            var anchorNode = new AnchorNode(anchor);

        //        //            var model = new TransformableNode(arFragment.TransformationSystem);
        //        //            model.SetParent(anchorNode);
        //        //            model.Renderable = bottleRenderable;

        //        //            //model.Select();
        //        //            if (augmentedImage.Name == "vertical.png")
        //        //            {
        //        //                var rotation90 = Quaternion.AxisAngle(new Vector3(1.0f, 0.0f, 0.0f), -90);
        //        //                model.LocalRotation = rotation90;
        //        //            }

        //        //            //model.LocalScale = new Vector3(0.2f, 0.2f, 0.2f);

        //        //            anchorNode.LocalScale = Vector3.One().Scaled(0.3f);

        //        //            this.arFragment.ArSceneView.Scene.AddChild(anchorNode);
        //        //            this.modelAdded = true;

        //        //        }
        //        //    }
        //        //}
        //    }
        //    catch (System.Exception ex)
        //    {
        //       //Mvx.IoCProvider.GetSingleton<IAnalyticsService>()?.LogException(ex);
        //    }
        //}


        /// <summary>
        /// Shows a message to the user
        /// </summary>
       private void ShowMessage(string message)
        {
            //Invo(() =>
            {
                try
                {
                    var activity = this.Context as Activity;
                    var view = activity.FindViewById(Android.Resource.Id.Content);
                    Snackbar snackBar = Snackbar.Make(view, message, Snackbar.LengthIndefinite);
                    //snackBar.SetActionTextColor(Android.Graphics.Color.White);
                    //snackBar.SetAction("Ok", action => { });
                    snackBar.SetDuration(6000);
                    snackBar.Show();
                }
                catch
                {
                    // Nothing to do
                }
            }///);
        }



        /// </inheritdoc>  
        private void LogDebug(object sender, LogDebugEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Args.Message))
            {
                return;
            }
            //Debug.WriteLine(e.Args.Message);
            //this.OnLogDebug?.Invoke(sender, e);
        }


        /// </inheritdoc>
        private void Error(object sender, SessionErrorEventArgs e)
        {
            SessionErrorEvent eventArgs = e?.Args;
            if (eventArgs == null)
            {
                //Debug.WriteLine("Azure Spatial Anchors reported an unspecified error.");
                return;
            }
            /*string message = $"{eventArgs.ErrorCode}: {eventArgs.ErrorMessage}";
            Debug.WriteLine(message);
            this.OnSessionError?.Invoke(sender, eventArgs);*/
        }


        private void StartLocatingAnchors()
        {
            // Only 1 active watcher at a time is permitted.
            //this.StopLocating();

            //return this.spatialAnchorsSession.CreateWatcher(locateCriteria);

            /*var nearbyLocateCriteria = new AnchorLocateCriteria()
            {
                
            }*/
            var nearbyLocateCriteria = new AnchorLocateCriteria();

            var nearAnchorCriteria = new NearAnchorCriteria
            {
                DistanceInMeters = 10,
                MaxResultCount = 10
                //SourceAnchor = this.anchorVisuals[this.anchorID].CloudAnchor
            };
            nearbyLocateCriteria.NearAnchor = nearAnchorCriteria;
            // Cannot run more than one watcher concurrently

            //this.StopWatcher();
            //this.anchorsToLocate = this.saveCount;

            StopLocatingAnchors();
            this.spatialAnchorsSession.CreateWatcher(nearbyLocateCriteria);

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
        
        private void OnAnchorLocated(object sender, AnchorLocatedEventArgs e)
        {
            var status = e.Args.Status;

            if (status == LocateAnchorStatus.AlreadyTracked
                    || status == LocateAnchorStatus.Located)
            {
                //this.anchorsToLocate--;
            }

            if (status == LocateAnchorStatus.Located)
            {
                ShowMessage("Anchor found");
            }
            else if (status == LocateAnchorStatus.Located)
            {
                AddLocatedAnchor(e.Args.Anchor);
            }

            /*this.RunOnUiThread(() =>
            {
                if (status == LocateAnchorStatus.AlreadyTracked)
                {
                    // Nothing to do since we've already rendered any anchors we've located.
                }
                else if (status == LocateAnchorStatus.Located)
                {
                    this.RenderLocatedAnchor(eventArgs.Anchor);
                }
                else if (status == LocateAnchorStatus.NotLocatedAnchorDoesNotExist)
                {
                    this.statusText.Text = "Anchor does not exist";
                }
            });*/
        }


        /// <summary>
        /// Called when it finish to located anchors
        /// </summary>       
        private void OnLocateAnchorsCompleted(object sender, LocateAnchorsCompletedEventArgs e)
        {
            StopLocatingAnchors();
            ShowMessage("Finished locating anchors");
            /*if (this.anchorsToLocate > 0)
            {
                // We didn't find all of the anchors.
                this.StopWatcher();
                this.RunOnUiThread(() =>
                {
                    this.statusText.Text = "Not all anchors were located. Check the logs for errors and\\or try again.";
                    this.actionButton.Visibility = ViewStates.Visible;
                    this.actionButton.Text = "Cleanup anchors";
                });
                this.currentDemoStep = DemoStep.End;

                return;
            }

            this.anchorsToLocate = 0;

            this.RunOnUiThread(() => this.statusText.Text = "Anchor located!");

            if (!this.basicDemo && this.currentDemoStep == DemoStep.LocateAnchor)
            {
                this.RunOnUiThread(() =>
                {
                    this.actionButton.Visibility = ViewStates.Visible;
                    this.actionButton.Text = "Look for anchors nearby";
                });
                this.currentDemoStep = DemoStep.LocateNearbyAnchors;
            }
            else
            {
                this.StopWatcher();
                this.RunOnUiThread(() =>
                {
                    this.actionButton.Visibility = ViewStates.Visible;
                    this.actionButton.Text = "Cleanup anchors";
                });
                this.currentDemoStep = DemoStep.End;
            }*/
        }



        private void AddLocatedAnchor(CloudSpatialAnchor anchor)
        {

            var anchorModel = new AnchorModel
            {
                AnchorNode = new AnchorNode(anchor.LocalAnchor),
            };
            anchorModel.AddToScene(modelRenderable, arFragment);
            var cloudAnchorIdentifier = anchorModel.CloudAnchor.Identifier;
            this.anchorVisuals[cloudAnchorIdentifier] = anchorModel;


            //AnchorVisual foundVisual = new AnchorVisual(anchor.LocalAnchor)
            //{
            //    CloudAnchor = anchor
            //};
            //foundVisual.AnchorNode.SetParent(this.arFragment.ArSceneView.Scene);
            //string cloudAnchorIdentifier = foundVisual.CloudAnchor.Identifier;
            //foundVisual.SetColor(foundColorMaterial);
            //foundVisual.AddToScene(this.arFragment);
            //this.anchorVisuals[cloudAnchorIdentifier] = foundVisual;
        }
    }
}