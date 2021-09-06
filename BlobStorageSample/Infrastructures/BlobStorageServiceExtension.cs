using Azure.Identity;
using Azure.Storage.Blobs;
using BlobStorageSample.Services;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlobStorageSample.Infrastructures
{
    public static class BlobStorageServiceExtension
    {
        public static IServiceCollection RegisterBlobStorageService(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<ApplicationSettings>(configuration.GetSection("ApplicationCredential"));
            var settings = configuration.GetSection("ApplicationCredential").Get<ApplicationSettings>();

            var credentials = new AzureCredentialsFactory().FromServicePrincipal(
                settings.ClientId,
                settings.ClientSecret,
                settings.TenantId,
                AzureEnvironment.AzureGlobalCloud);
            var credential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret);
            serviceCollection.AddScoped(_ => new BlobContainerClientBuilder(settings.StorageAccountUrl, settings.Container)
                    .WithClientCredential(settings.TenantId, settings.ClientId, settings.ClientSecret)
                    .Build());
            serviceCollection.AddScoped(_ => new BlobServiceClient(settings.StorageAccountUrl, credential));

            serviceCollection.AddScoped<IBlobStorageService, BlobStorageService>();

            return serviceCollection;
        }
    }
}
