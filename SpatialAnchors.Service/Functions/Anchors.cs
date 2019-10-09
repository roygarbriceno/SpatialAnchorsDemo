namespace SpatialAnchors.Service.Functions
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SpatialAnchors.Service.DataAccess;
    using SpatialAnchors.Models;
    using SpatialAnchors.Service.Entities;

    /// <summary>
    /// Anchors API
    /// Used to store and retrieve the anchors details
    /// </summary>
    public class Anchors
    {
        private readonly IStorageDataAccess dataAccess;

        /// <summary>
        /// Receives by DI the dataAccess
        /// </summary>        
        public Anchors(IStorageDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }


        /// <summary>
        /// Adds / update a new anchor
        /// </summary>        
        [FunctionName("SubmitAnchorAsync")]
        public async Task<IActionResult> SubmitAnchorAsync(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "v1/anchors")] HttpRequest request,
            CancellationToken token, ILogger logger)
        {
            try
            {
                var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                var item = JsonConvert.DeserializeObject<Anchor>(requestBody);
                 await this.dataAccess.AddUpdateAsync<AnchorEntity>(
                    "Anchors", new AnchorEntity
                    {
                        AnchorId = item.AnchorId,
                        PartitionKey = "Anchors",
                        RowKey = item.AnchorId
                    });
                return new OkObjectResult(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex, nameof(SubmitAnchorAsync));
            }
            return new BadRequestResult();
        }


        /// <summary>
        /// Returns the available anrchovs
        /// </summary>        
        [FunctionName("GetAnchorAsync")]
        public async Task<IActionResult> GetAnchorsAsync(
            [HttpTrigger(AuthorizationLevel.Function, "GET", Route = "v1/anchors")] HttpRequest request,
            CancellationToken token, ILogger logger)
        {
            try
            {
                var entites = await this.dataAccess.GetItemsAsync<AnchorEntity>("Anchors");
                return new OkObjectResult(entites.Select(x=> new Anchor
                {
                    AnchorId = x.AnchorId
                }));              
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex, nameof(GetAnchorsAsync));
            }
            return new BadRequestResult();
        }
    }
}
