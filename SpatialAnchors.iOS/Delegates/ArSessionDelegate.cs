using ARKit;
using Foundation;
using SceneKit;
using SpatialAnchors.Core.ViewModels;


namespace SpatialAnchors.iOS.Delegates
{
    /// <summary>
    /// ARKit session delegate
    /// </summary>
    public class ArSessionDelegate : ARSCNViewDelegate
    {
        private readonly ARSCNView sceneView;
        private readonly AnchorsViewModel viewModel;

        public ArSessionDelegate(ARSCNView sceneView, AnchorsViewModel viewmodel)
        {
            this.sceneView = sceneView;
            this.viewModel = viewmodel;
        }

        public override void DidUpdateNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {         
        }


        public override void WasInterrupted(ARSession session)
        {
            base.WasInterrupted(session);        
        }


        public override void InterruptionEnded(ARSession session)
        {
            base.InterruptionEnded(session);
        }


        public override void DidFail(ARSession session, NSError error)
        {
            if (error.Code == 102)
            {
                session.Pause();
                session.Run(new ARWorldTrackingConfiguration
                {
                    AutoFocusEnabled = true,
                    PlaneDetection = ARPlaneDetection.Horizontal,
                    LightEstimationEnabled = true,
                    WorldAlignment = ARWorldAlignment.Gravity
                }, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);

            }
        }


        /// <summary>
        ///  Note: Always a super-tricky thing in ARKit : must get rid of the managed reference to the Frame object ASAP.
        /// </summary>
        public override void WillRenderScene(ISCNSceneRenderer renderer, SCNScene scene, double timeInSeconds)
        {
            using (var frame = this.sceneView.Session?.CurrentFrame)
            {
                if (frame == null) return;
                this.viewModel.ProcessFrame(frame);                            
            }
        }
    }
}