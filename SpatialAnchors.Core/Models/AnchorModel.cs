using System;
using System.Collections.Generic;
using System.Text;

namespace SpatialAnchors.Core.Models
{
    public abstract class BaseAnchorModel<T, V>
    {
        public T AnchorNode { get; set; }

        public V CloudAnchor { get; set; }
     
    }
}
