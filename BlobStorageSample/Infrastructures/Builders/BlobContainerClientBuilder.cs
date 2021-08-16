using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using System;

namespace BlobStorageSample.Infrastructures
{
    public sealed class BlobContainerClientBuilder : IBlobContainerClientBuilder
    {
        private readonly Uri _storageAccountUrl;
        private readonly string _container;
        private readonly Uri _endpoint;

        public BlobContainerClientBuilder(Uri storageAccountUrl, string container)
        {
            _storageAccountUrl = storageAccountUrl;
            _container = container;
            _endpoint = new Uri(storageAccountUrl, container);
        }

        public BlobContainerClientBuilder(string storageAccountUrl, string container)
        {
            _storageAccountUrl = new Uri(storageAccountUrl);
            _container = container;
            _endpoint = new Uri(_storageAccountUrl, container);
        }

        public IBlobContainerClientFinalStep WithClientCredential(string tenantId, string clientId, string clientSecret)
        {
            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            return new BlobContainerClientFinalStep(_endpoint, credential);
        }

        public IBlobContainerClientFinalStep WithSasCredential(string sasToken)
        {
            var credential = new AzureSasCredential(sasToken);
            return new BlobContainerClientFinalStep(_endpoint, credential);
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
