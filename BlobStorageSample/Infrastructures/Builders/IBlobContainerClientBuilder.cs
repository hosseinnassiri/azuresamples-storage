using Azure.Storage.Blobs;

namespace BlobStorageSample.Infrastructures
{
    public interface IBlobContainerClientBuilder
    {
        IBlobContainerClientFinalStep WithSasCredential(string sasToken);
        IBlobContainerClientFinalStep WithClientCredential(string tenantId, string clientId, string clientSecret);
    }

    public interface IBlobContainerClientFinalStep
    {
        BlobContainerClient Build();
    }
}
