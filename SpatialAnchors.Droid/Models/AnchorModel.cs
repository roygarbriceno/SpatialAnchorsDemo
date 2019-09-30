using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.AR.Sceneform;
using Google.AR.Sceneform.Rendering;
using Google.AR.Sceneform.UX;
using Microsoft.Azure.SpatialAnchors;
using SpatialAnchors.Core.Models;

namespace SpatialAnchors.Droid.Models
{
    public class AnchorModel : BaseAnchorModel<AnchorNode, CloudSpatialAnchor>
    {
        public void AddToScene(Renderable rendereable, ArFragment arFragment)
        {
            this.AnchorNode.SetParent(arFragment.ArSceneView.Scene);
            var model = new TransformableNode(arFragment.TransformationSystem);
            model.SetParent(this.AnchorNode);
            model.Renderable = rendereable;
            model.Select();
        }

    }
}