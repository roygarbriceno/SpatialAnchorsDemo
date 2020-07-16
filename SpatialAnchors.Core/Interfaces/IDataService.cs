namespace SpatialAnchors.Core.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;


    /// <summary>
    /// API calls interface
    /// </summary>
    public interface IDataService
    {

        /// <summary>
        /// Saves an anchor
        /// </summary>        
        Task<bool> SaveAnchorAsync(Models.Anchor anchor);


        /// <summary>
        /// Gets tha available anchors
        /// </summary>
        /// <returns></returns>
        Task<List<Models.Anchor>> GetAnchorsAsync();

        /// <inheritdoc/>        
        Task<byte[]> GetModel(string plataform);
    }
}
