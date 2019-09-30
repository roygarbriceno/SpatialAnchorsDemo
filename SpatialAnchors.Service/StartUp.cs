using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpatialAnchors.Service.DataAccess;
using System;


[assembly: FunctionsStartup(typeof(SpatialAnchors.Service.Startup))]
namespace SpatialAnchors.Service
{
    /// <summary>
    /// Function startup 
    /// Register all requiered components by DI
    /// </summary>
    public class Startup : FunctionsStartup
    {
        private readonly IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(Environment.CurrentDirectory)
           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables()
           .Build();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IStorageDataAccess>(s =>
            {
                return new StorageDataAccess(configuration);
            });
        }
    }
}
