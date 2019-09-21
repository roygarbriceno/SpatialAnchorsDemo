namespace SpatialAnchors.Core.Services
{
    using MvvmCross.Plugin.JsonLocalization;
    using System.Collections.Generic;


    /// <summary>
    /// Constants used for localizations values
    /// </summary>
    public static class TextProviderConstants
    {
        public const string RootFolderForResources = "Languages";
        public const string GeneralNamespace = "MobileTemplate";
        public const string ClassName = "ViewModels";
        public const string FileName = "Texts";
    }


    /// <summary>
    /// Json localization provider
    /// </summary>
    public class TextProviderBuilder : MvxTextProviderBuilder
    {
        protected override IDictionary<string, string> ResourceFiles => new Dictionary<string, string>
        {
            {
                TextProviderConstants.ClassName,
                TextProviderConstants.FileName
            }
        };

        public TextProviderBuilder()
            : base(TextProviderConstants.GeneralNamespace, TextProviderConstants.RootFolderForResources)
        {
        }
    }
}
