namespace SpatialAnchors.Service.Entities
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class AnchorEntity : TableEntity
    {
        public string AnchorId { get; set; }
    }
}
