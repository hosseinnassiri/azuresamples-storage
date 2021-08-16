using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using System;

namespace BlobStorageSample.Infrastructures
{
    public sealed class BlobContainerClientBuilder : IBlobContainerClientBuilder
    {
        private readonly Uri _storageAccountUrl;

        public BlobContainerClientBuilder(Uri storageAccountUrl)
        {
            _storageAccountUrl = storageAccountUrl;
        }

        public BlobContainerClientBuilder(string storageAccountUrl)
        {
            _storageAccountUrl = new Uri(storageAccountUrl);
        }

        public IBlobContainerClientFinalStep WithClientCredential(string tenantId, string clientId, string clientSecret)
        {
            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            return new BlobContainerClientFinalStep(_storageAccountUrl, credential);
        }

        public IBlobContainerClientFinalStep WithSasCredential(string sasToken)
        {
            var credential = new AzureSasCredential(sasToken);
            return new BlobContainerClientFinalStep(_storageAccountUrl, credential);
        }

        private sealed class BlobContainerClientFinalStep : IBlobContainerClientFinalStep
        {
            private readonly BlobContainerClient _blobContainerClient;
            private readonly Uri _storageAccountUrl;

            public BlobContainerClientFinalStep(Uri storageAccountUrl, ClientSecretCredential clientCredential)
            {
                _storageAccountUrl = storageAccountUrl;
                _blobContainerClient = new BlobContainerClient(_storageAccountUrl, clientCredential);
            }

            public BlobContainerClientFinalStep(Uri storageAccountUrl, AzureSasCredential sasCredential)
            {
                _storageAccountUrl = storageAccountUrl;
                _blobContainerClient = new BlobContainerClient(_storageAccountUrl, sasCredential);
            }

            public BlobContainerClient Build() => _blobContainerClient;
        }
    }
}
