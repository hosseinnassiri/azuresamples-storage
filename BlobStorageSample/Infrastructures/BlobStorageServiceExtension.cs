using Azure.Identity;
using Azure.Storage.Blobs;
using BlobStorageSample.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlobStorageSample.Infrastructures
{
    public static class BlobStorageServiceExtension
    {
        public static IServiceCollection RegisterBlobStorageService(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<ApplicationSettings>(configuration.GetSection("ApplicationCredential"));
            var settings = configuration.GetSection("ApplicationCredential").Get<ApplicationSettings>();
            var credential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret);
            serviceCollection.AddScoped(_ => new BlobContainerClientBuilder(new Uri(settings.StorageAccountUrl, settings.RootContainer))
                    .WithClientCredential(settings.TenantId, settings.ClientId, settings.ClientSecret)
                    .Build());
            serviceCollection.AddScoped(_ => new BlobServiceClient(settings.StorageAccountUrl, credential));

            serviceCollection.AddScoped<IBlobStorageService, BlobStorageService>();

            return serviceCollection;
        }
    }
}
