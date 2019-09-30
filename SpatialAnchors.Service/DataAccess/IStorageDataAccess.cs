using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace SpatialAnchors.Service.DataAccess
{
    public interface IStorageDataAccess
    {
        Task AddUpdateAsync<EntityType>(string storageTable, EntityType entity) where EntityType : TableEntity, new();
        Task AddUpdateAsync<EntityType>(string storageTable, IEnumerable<EntityType> entities) where EntityType : TableEntity, new();
        Task<IEnumerable<EntityType>> GetItemsAsync<EntityType>(string storageTable, string partitionKey = "", int maxCount = -1) where EntityType : TableEntity, new();
    }
}