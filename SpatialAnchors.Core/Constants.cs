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
        public const string SpatialAnchorsAccountId = "8b9453bf-3aa2-46cb-9284-5faef033cfa2";

        /// <summary>
        /// Set this value to the spatial anchors key shown on the azure portal
        /// </summary>
        public const string SpatialAnchorsAccountKey = "RBVocjn8l7dy6GymqpDNRg2N7L5lvv9a1rF4R4ewmXA=";

        /// <summary>
        /// Set this value to the URI where the API is published
        /// </summary>
        public const string ServiceUri = "https://spatialanchorsdemo.azurewebsites.net/api/v1/";

        public const string ModelsUri = "https://spatialanchorsdemo.blob.core.windows.net/models";

        /// <summary>
        /// Set this value to key used by the API (shown on the azure portal)
        /// </summary>
        public const string XFunctionsKey = "vOXa257CApcaBlBxyeveOcRLzmqjH2RorK6UayZ9ly/Kdpj6CGwXXg==";

        public const string XFunctionsKeyHeader = "x-functions-key";

        public const string SaveAnchorsUri = ServiceUri + "Anchors";

        public const string GetAnchorsUri = ServiceUri + "Anchors";


        public const string GetModelsUri = ServiceUri + "Models";
    }
}
