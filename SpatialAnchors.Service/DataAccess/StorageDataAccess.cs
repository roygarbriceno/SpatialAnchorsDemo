namespace SpatialAnchors.Service.DataAccess
{
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Data access methods
    /// </summary>
    public class StorageDataAccess : IStorageDataAccess
    {
        private readonly string connectionString;

        public StorageDataAccess(IConfigurationRoot configuration)
        {
            this.connectionString = configuration.GetConnectionString("StorageConnectionString");
        }

        /// <summary>
        /// Gets the cloud table reference
        /// </summary>
        internal async Task<CloudTable> GetTableReferenceAsync(string storageTable)
        {
            var storageAccount = CloudStorageAccount.Parse(this.connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(storageTable);
            await table.CreateIfNotExistsAsync();
            return table;
        }


        /// <summary>
        /// Add or updates an entity
        /// </summary>
        public async Task AddUpdateAsync<EntityType>(string storageTable, EntityType entity)
            where EntityType : TableEntity, new()
        {
            var table = await GetTableReferenceAsync(storageTable);
            var insertOperation = TableOperation.InsertOrReplace(entity);
            await table.ExecuteAsync(insertOperation);
        }


        /// <summary>
        /// Add or updates a batch of entities
        /// </summary>
        public async Task AddUpdateAsync<EntityType>(string storageTable, IEnumerable<EntityType> entities)
            where EntityType : TableEntity, new()
        {
            var batchOperation = new TableBatchOperation();
            var lastPartitionKey = string.Empty;
            var table = await GetTableReferenceAsync(storageTable);

            var orderedEntities = entities.OrderBy(e => e.PartitionKey).ToList();

            foreach (var entity in orderedEntities)
            {
                if (lastPartitionKey == string.Empty)
                {
                    lastPartitionKey = entity.PartitionKey;
                }

                if (lastPartitionKey == entity.PartitionKey)
                {
                    batchOperation.InsertOrReplace(entity);
                }
                else
                {
                    await table.ExecuteBatchAsync(batchOperation);
                    batchOperation.Clear();
                    batchOperation = new TableBatchOperation();
                    lastPartitionKey = entity.PartitionKey;
                    batchOperation.InsertOrReplace(entity);
                }

                if (batchOperation.Count != 100) continue;

                await table.ExecuteBatchAsync(batchOperation);

                batchOperation.Clear();
                batchOperation = new TableBatchOperation();
                lastPartitionKey = string.Empty;
            }

            if (batchOperation.Count > 0)
            {
                await table.ExecuteBatchAsync(batchOperation);
                batchOperation.Clear();
            }
        }



        /// <summary>
        /// Returns data from a specific table in Azure
        /// </summary>
        public async Task<IEnumerable<EntityType>> GetItemsAsync<EntityType>(
            string storageTable, string partitionKey = "", int maxCount = -1)
        where EntityType : TableEntity, new()
        {
            TableQuery<EntityType> tableQuery;

            if (partitionKey.Length > 0)
            {
                var filterCondition =
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, $"{partitionKey}");
                tableQuery = new TableQuery<EntityType>().Where(filterCondition);
            }
            else
            {
                tableQuery = new TableQuery<EntityType>();
            }

            return await GetItemsAsync(storageTable, tableQuery, maxCount);
        }



        /// <summary>
        /// Implementation of azure storage get 
        /// </summary>
        protected async Task<IEnumerable<EntityType>> GetItemsAsync<EntityType>(
            string storageTable, TableQuery<EntityType> tableQuery, int maxCount = -1)
            where EntityType : TableEntity, new()
        {
            var items = new List<EntityType>();
            TableContinuationToken continuationToken = null;

            var table = await GetTableReferenceAsync(storageTable);

            do
            {
                var tableQueryResult = await table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
                continuationToken = tableQueryResult.ContinuationToken;
                items.AddRange(tableQueryResult.Results);
                if (maxCount != -1)
                {
                    if (items.Count > maxCount) break;
                }

            } while (continuationToken != null);

            return items;
        }


        /// <summary>
        /// Returns all items froma a certain table
        /// </summary>
        /// <returns></returns>
        internal async Task<T> GetItemAsync<T>(string storageTable, string partitionKey, string rowKey) where T : TableEntity, new()
        {
            var table = await GetTableReferenceAsync(storageTable);
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(retrieveOperation);
            return (T)result.Result;
        }

        /// <summary>
        /// Deletes an item
        /// </summary>        
        internal async Task<bool> DeleteAsync<T>(string storageTable, string partitionKey, string rowKey) where T : TableEntity, new()
        {
            var table = await GetTableReferenceAsync(storageTable);
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(retrieveOperation);
            var item = (T)result.Result;

            var deleteOperation = TableOperation.Delete(item);
            await table.ExecuteAsync(deleteOperation);
            return true;
        }
    }
}
