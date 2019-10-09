namespace SpatialAnchors.Core.Services
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using SpatialAnchors.Core.Interfaces;


    /// <summary>
    /// Data access service
    /// </summary>
    public class DataService : IDataService
    {        
        private static HttpClient httpClient;


        /// <summary>
        /// Initialization
        /// </summary>
        public DataService()
        {           
        }


       
        /// <summary>
        /// Creates and configures the HTTP client
        /// </summary>
        private void CreateHttpClient()
        {
            // Disables the default cache
            if (httpClient != null)
            {
                httpClient.DefaultRequestHeaders.IfModifiedSince = DateTimeOffset.Now;
            }
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.IfModifiedSince = DateTimeOffset.Now;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(Constants.XFunctionsKeyHeader, Constants.XFunctionsKey);
            httpClient.Timeout = new TimeSpan(0, 0, 5, 0);
        }


        /// <summary>
        /// Post data 
        /// </summary>
        protected async Task<T> PostAsync<T>(object postData, string uri, JsonSerializerSettings settings = null)
        {
            CreateHttpClient();
            var data = JsonConvert.SerializeObject(postData);
            var contentPost = new StringContent(data, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(uri, contentPost);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Error in PostAsync: {result.StatusCode}");
            }
            var response = await result.Content.ReadAsStringAsync();
            var resultData = JsonConvert.DeserializeObject<T>(response, settings);
            return resultData;
        }


        /// <summary>
        /// Executes a GET method to the service
        /// </summary>
        protected async Task<T> GetAsync<T>(string uri, JsonSerializerSettings settings = null)
        {
            CreateHttpClient();
            var result = await httpClient.GetAsync(uri);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Error in GetAsync: {result.StatusCode}");
            }
            var response = await result.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<T>(response);
            return data;
        }


        /// <inheritdoc/>        
        public async Task<bool> SaveAnchorAsync(SpatialAnchors.Models.Anchor anchor)
        {                        
            return await PostAsync<bool>(anchor, Constants.SaveAnchorsUri);            
        }


        /// <inheritdoc/>        
        public async Task<List<SpatialAnchors.Models.Anchor>> GetAnchorsAsync()
        {            
            return await GetAsync<List<SpatialAnchors.Models.Anchor>>(Constants.GetAnchorsUri);            
        }

    }
}


