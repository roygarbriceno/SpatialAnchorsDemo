namespace SpatialAnchors.Core.Arguments
{
    /// <summary>
    /// Parameter for the AR ViwModel
    /// </summary>
    public class SpatialAnchorsParameter
    {
        public SpatialAnchorsMode Mode { get; set; }

        public Models.Anchor[] Anchors { get; set; }
    }
}
