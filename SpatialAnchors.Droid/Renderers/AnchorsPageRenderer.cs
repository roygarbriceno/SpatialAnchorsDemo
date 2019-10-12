using Android.App;
using Android.Content;
using Android.Views;
using Google.AR.Sceneform.UX;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using SpatialAnchors.Core.ViewModels;
using SpatialAnchors.Core.Pages;
using SpatialAnchors.Droid.Renderers;
using SpatialAnchors.Droid.Fragments;
using System;

[assembly: ExportRenderer(typeof(AnchorsPage), typeof(AnchorsPageRenderer))]
namespace SpatialAnchors.Droid.Renderers
{
    /// <summary>
    /// Andriod Renderer for our AR session.
    /// Uses an ArFrament and passes the session to the ViewModel
    /// </summary>
    public class AnchorsPageRenderer : PageRenderer
    {        
        private ArFragment arFragment;        
        private Android.Views.View view;
        private AnchorsViewModel viewModel;
        

        /// <summary>
        /// Constructor
        /// </summary>        
        public AnchorsPageRenderer(Context context) : base(context)
        {
            this.AutoPackage = false;           
        }


        /// <summary>
        /// Gets a reference to the ViewModel and the ArFragment
        /// </summary>       
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Page> e)
        {
            try
            {
                base.OnElementChanged(e);
                var activity = this.Context as Activity;
                this.viewModel = this.Element.BindingContext as AnchorsViewModel;

                // Gets the view and setups the AR fragment 
                this.view = activity.LayoutInflater.Inflate(Resource.Layout.AnchorsLayout, this, false);
                AddView(this.view);
                
                this.arFragment = activity.GetFragmentManager().FindFragmentById(Resource.Id.anchors_fragment) as AnchorsFragment;
                if (this.arFragment != null)
                {                    
                    this.arFragment.ArSceneView.Scene.Update += (_, args) =>
                    {                     
                        // Passes the frame to the viewmodels
                        // this's needed for the spatial anchors session
                        this.viewModel.ProcessFrame(this.arFragment.ArSceneView.ArFrame);
                    };                   

                    // Starts the session
                    this.viewModel.StartSession(this.Context, this.arFragment);                   
                }
            }
            catch (Exception ex)
            {             
                this.viewModel.ShowMessage("UnableToStartArSession", ex.Message);
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
            this.viewModel.StopSession();
            var activity = this.Context as Activity;            
            activity.GetFragmentManager().BeginTransaction().Remove(this.arFragment).Commit();            
        }
    }
}