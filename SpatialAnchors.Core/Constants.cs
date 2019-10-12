namespace SpatialAnchors.Core
{
    /// <summary>
    /// Constants used for in the app
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Set this value to the spatial anchors account id shown on the azure portal
        /// </summary>
        public const string SpatialAnchorsAccountId = "fe723cf9-aaed-455f-bd36-322a14657249";

        /// <summary>
        /// Set this value to the spatial anchors key shown on the azure portal
        /// </summary>
        public const string SpatialAnchorsAccountKey = "o5dc40N0mFQ1YDFqhddcTf3WijFf9X4vylVmo+Nu5E0=";

        /// <summary>
        /// Set this value to the URI where the API is published
        /// </summary>
        public const string ServiceUri = "https://spatialanchorsdemo.azurewebsites.net/api/v1/";

        /// <summary>
        /// Set this value to key used by the API (shown on the azure portal)
        /// </summary>
        public const string XFunctionsKey = "vOXa257CApcaBlBxyeveOcRLzmqjH2RorK6UayZ9ly/Kdpj6CGwXXg==";

        public const string XFunctionsKeyHeader = "x-functions-key";

        public const string SaveAnchorsUri = ServiceUri + "Anchors";

        public const string GetAnchorsUri = ServiceUri + "Anchors";
    }
}
