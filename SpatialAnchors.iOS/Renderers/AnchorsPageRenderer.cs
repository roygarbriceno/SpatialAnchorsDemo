using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARKit;
using Foundation;
using SpatialAnchors.Core.Pages;
using SpatialAnchors.Core.ViewModels;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AnchorsPage), typeof(ARPageRenderer))]
namespace SpatialAnchors.iOS.Renderers
{
    /// <summary>
    /// Page Renderer to display AR Screen View from Forms Code, implementing AR ScreenView Delegate
    /// </summary>
    public class ARPageRenderer : PageRenderer, IARSCNViewDelegate
    {
        private ARSCNView sceneView;
        private AnchorsViewModel viewModel;

        public override bool ShouldAutorotate() => true;


        public ARPageRenderer()
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
                Delegate = new ARDelegate(this),
                UserInteractionEnabled = true,
                DebugOptions = ARSCNDebugOptions.ShowFeaturePoints,                
            };
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
                base.ViewWillAppear(animated);
                var configuration = new ARWorldTrackingConfiguration
                {
                    PlaneDetection = ARPlaneDetection.Horizontal
                };
                this.sceneView.Session.Run(configuration, ARSessionRunOptions.RemoveExistingAnchors));
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
    }
}