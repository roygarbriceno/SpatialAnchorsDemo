using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARKit;
using CoreGraphics;
using Foundation;
using MvvmCross;
using OpenTK;
using SceneKit;
using SpatialAnchors.Core;
using SpatialAnchors.Core.Interfaces;
using SpatialAnchors.Core.Pages;
using SpatialAnchors.Core.ViewModels;
using SpatialAnchors.iOS.Delegates;
using SpatialAnchors.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ExportRenderer(typeof(AnchorsPage), typeof(AnchorsPageRenderer))]
namespace SpatialAnchors.iOS.Renderers
{
    /// <summary>
    /// Page Renderer to display AR Screen View from Forms Code, implementing AR ScreenView Delegate
    /// </summary>
    public class AnchorsPageRenderer : PageRenderer, IARSCNViewDelegate
    {
        private ARSCNView sceneView;
        private AnchorsViewModel viewModel;

        public override bool ShouldAutorotate() => true;


        public AnchorsPageRenderer()
        {
            this.sceneView = new ARSCNView();
        }


        /// <summary>
        /// Setup the frame for the sceneview
        /// </summary>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.viewModel = this.Element.BindingContext as AnchorsViewModel;
           
            this.sceneView = new ARSCNView
            {
                Frame = this.View.Frame,              
                UserInteractionEnabled = true,
            };

            this.sceneView.Delegate = new ArSessionDelegate(this.sceneView, this.viewModel);
            this.View.AddSubview(this.sceneView);

        }


        /// <summary>
        /// Configures AR kit
        /// </summary>        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {
                this.sceneView.Session.Run(new ARWorldTrackingConfiguration
                {
                    AutoFocusEnabled = true,
                    PlaneDetection = ARPlaneDetection.Horizontal,
                    LightEstimationEnabled = true,
                    WorldAlignment = ARWorldAlignment.GravityAndHeading
                }, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);

                this.viewModel.StartSession(null, this.sceneView);

            }
            catch (Exception ex)
            {
                this.viewModel.ShowMessage("Ërror", ex.Message);
            }

        }


        /// <summary>
        /// Pause the AR scene
        /// </summary>        
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            this.sceneView.Session.Pause();
        }


        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            this.View.EndEditing(true);
            base.TouchesBegan(touches, evt);

            if (this.viewModel.Mode != SpatialAnchorsMode.AddAnchors) return;         
            var touch = touches.AnyObject as UITouch;
            if (touch != null)
            {
                var touchLocation = touch.LocationInView(this.sceneView);
                var worldPos = WorldPositionFromHitTest(touchLocation);
                if (worldPos.Item1.HasValue)
                {
                    this.TryHitTestFromTouchPoint(touchLocation,  out NMatrix4 worldTransform);
                    Mvx.IoCProvider.GetSingleton<ISpatialAnchorsService>()?.PlaceModel(worldTransform);                    
                }
             
            }
           
        }

        /// <summary>
        /// Getting world position from touch hit
        /// </summary>
        Tuple<SCNVector3?, ARAnchor> WorldPositionFromHitTest(CGPoint pt)
        {
            //Hit test against existing anchors
            var hits = this.sceneView.HitTest(pt, ARHitTestResultType.ExistingPlaneUsingExtent);
            if (hits != null && hits.Length > 0)
            {
                var anchors = hits.Where(r => r.Anchor is ARPlaneAnchor);
                if (anchors.Count() > 0)
                {
                    var first = anchors.First();
                    var pos = PositionFromTransform(first.WorldTransform);
                    return new Tuple<SCNVector3?, ARAnchor>(pos, (ARPlaneAnchor)first.Anchor);
                }
            }
            return new Tuple<SCNVector3?, ARAnchor>(null, null);
        }


        private SCNVector3 PositionFromTransform(NMatrix4 xform)
        {
            return new SCNVector3(xform.M14, xform.M24, xform.M34);
        }


        /// <summary>
        /// Hit test against existing anchors
        /// </summary>        
        private bool TryHitTestFromTouchPoint(CGPoint pt, out NMatrix4 worldTransform)
        {
            
            ARHitTestResult[] hits = this.sceneView.HitTest(pt, ARHitTestResultType.FeaturePoint);
            if (hits != null && hits.Length > 0)
            {
                ARHitTestResult hit = hits.FirstOrDefault();
                if (hit != null)
                {
                    worldTransform = hit.WorldTransform;
                    return true;
                }
            }
            worldTransform = default;
            return false;
        }
    }
}