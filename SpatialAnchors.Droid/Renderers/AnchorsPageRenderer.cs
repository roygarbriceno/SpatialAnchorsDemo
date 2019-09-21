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

[assembly: ExportRenderer(typeof(AnchorsPage), typeof(AnchorsPageRenderer))]
namespace SpatialAnchors.Droid.Renderers
{
    //here is my very simple implementation of IFunction interface
    internal class Foo : Java.Lang.Throwable, IFunction
    {
        public Foo(Action action) { }

        public Java.Lang.Object Apply(Java.Lang.Object t)
        {
            return t;
        }
    }

    /// <summary>
    /// Page Renderer for our AR session
    /// </summary>
    public class AnchorsPageRenderer : PageRenderer, IOnUpdateListener//, IConsumer
    {
        private readonly Context context;
        private ArFragment arFragment;
        private ModelRenderable modelRenderable;
        private Android.Views.View view;
        private AnchorsViewModel viewModel;
        protected Config config;
        protected bool modelAdded = false;
        private IMvxTextProviderBuilder textProviderBuilder;

      

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
             
                this.view = activity.LayoutInflater.Inflate(Resource.Layout.AnchorsLayout, this, false);
                AddView(this.view);

                this.textProviderBuilder = Mvx.IoCProvider.GetSingleton<IMvxTextProviderBuilder>();
                this.arFragment = activity.GetFragmentManager().FindFragmentById(Resource.Id.anchors_fragment) as AnchorsFragment;

                if (this.arFragment != null)
                {
                    this.arFragment.ArSceneView.Scene.AddOnUpdateListener(this);
                    ModelRenderable.InvokeBuilder().SetSource(this.context, Resource.Raw.andy).Build(renderable =>
                    {
                        modelRenderable = renderable;
                    });
                    arFragment.TapArPlane += OnTapArPlane;
                }
            }
            catch (System.Exception ex)
            {
                // Show a message to the user
                ShowUserMessage("ArNotAvailable");
             }
        }

        /// <summary>
        /// Adds a model whe the user taps on the plane
        /// </summary>        
        private void OnTapArPlane(object sender, BaseArFragment.TapArPlaneEventArgs e)
        {
            if (modelRenderable == null) return;

            // Create the Anchor.
            var anchor = e.HitResult.CreateAnchor();
            var anchorNode = new AnchorNode(anchor);
            anchorNode.SetParent(arFragment.ArSceneView.Scene);

            // Create the transformable andy and add it to the anchor.
            var andy = new TransformableNode(arFragment.TransformationSystem);
            andy.SetParent(anchorNode);
            andy.Renderable = modelRenderable;
            andy.Select();
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


        /// <summary>
        /// Checks the update frame to track the images
        /// </summary>        
        public void OnUpdate(FrameTime frameTime)
        {

            try
            {
                //if (this.arFragment != null)
                //{
                //    var frame = this.arFragment.ArSceneView.ArFrame;
                //    var updatedAugmentedImages = frame.GetUpdatedTrackables(Java.Lang.Class.FromType(typeof(AugmentedImage)));
                //    foreach (var image in updatedAugmentedImages)
                //    {
                //        // Adds a model over the picture
                //        var augmentedImage = (AugmentedImage)image;
                //        if (augmentedImage.TrackingState == TrackingState.Tracking && !this.modelAdded)
                //        {
                //            var pose = augmentedImage.CenterPose;
                //            var anchor = augmentedImage.CreateAnchor(augmentedImage.CenterPose);
                //            var anchorNode = new AnchorNode(anchor);

                //            var model = new TransformableNode(arFragment.TransformationSystem);
                //            model.SetParent(anchorNode);
                //            model.Renderable = bottleRenderable;

                //            //model.Select();
                //            if (augmentedImage.Name == "vertical.png")
                //            {
                //                var rotation90 = Quaternion.AxisAngle(new Vector3(1.0f, 0.0f, 0.0f), -90);
                //                model.LocalRotation = rotation90;
                //            }

                //            //model.LocalScale = new Vector3(0.2f, 0.2f, 0.2f);

                //            anchorNode.LocalScale = Vector3.One().Scaled(0.3f);

                //            this.arFragment.ArSceneView.Scene.AddChild(anchorNode);
                //            this.modelAdded = true;

                //        }
                //    }
                //}
            }
            catch (System.Exception ex)
            {
               //Mvx.IoCProvider.GetSingleton<IAnalyticsService>()?.LogException(ex);
            }
        }


        /// <summary>
        /// Shows a message to the user
        /// </summary>
        /// <param name="message"></param>
        private void ShowUserMessage(string message)
        {
            try
            {
                var text = this.textProviderBuilder.TextProvider.GetText(
                    TextProviderConstants.GeneralNamespace, TextProviderConstants.ClassName, message);

                var activity = this.Context as Activity;
                var view = activity.FindViewById(Android.Resource.Id.Content);
                Snackbar snackBar = Snackbar.Make(view, text, Snackbar.LengthIndefinite);
                snackBar.SetActionTextColor(Android.Graphics.Color.White);
                snackBar.SetAction("Ok", action => { });
                snackBar.SetDuration(6000);
                snackBar.Show();
            }
            catch
            {
                // Nothing to do
            }
        }

     
    }
}