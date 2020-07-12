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
    /// Used to store and retrieve the models files
    /// </summary>
    public class Models
    {
        private readonly IStorageDataAccess dataAccess;

        /// <summary>
        /// Receives by DI the dataAccess
        /// </summary>        
        public Models(IStorageDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }



        /// <summary>
        /// Returns the model file
        /// </summary>        
        [FunctionName("GetModelAsync")]
        public async Task<IActionResult> GetModelAsync(
            [HttpTrigger(AuthorizationLevel.Function, "GET", Route = "v1/models/{platform}/{model}")] HttpRequest request,
            string platform, string model, CancellationToken token, ILogger logger)
        {
            try
            {
                var data = new byte[0];
                using (var stream = await this.dataAccess.GetModelFileAsync(platform, model))
                {
                    data = new byte[stream.Length];
                    stream.Read(data, 0, (int)stream.Length);                    
                }
                return new OkObjectResult(data);              
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex, nameof(GetModelAsync));
            }
            return new BadRequestResult();
        }
    }
}
