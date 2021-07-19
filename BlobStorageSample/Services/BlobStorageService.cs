using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BlobStorageSample.Services
{
    public sealed class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(BlobContainerClient containerClient, BlobServiceClient blobServiceClient)
        {
            _containerClient = containerClient;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<IReadOnlyList<string>> GetAllFileNamesAsync(CancellationToken cancellationToken = default)
        {
            var blobs = _containerClient.GetBlobsAsync(cancellationToken: cancellationToken);
            var list = new List<string>();
            await foreach (BlobItem blob in blobs)
            {
                list.Add(blob.Name);
            }
            return list;
        }

        public async Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken = default)
        {
            return await _containerClient.GetBlobClient(fileName).ExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<byte[]> DownloadAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);

            using var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

            return stream.ToArray();
        }

        public async Task UploadAsync(string fileName, Stream fileStream, CancellationToken cancellationToken = default)
        {
            if (await FileExistsAsync(fileName, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(fileStream, overwrite: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                return;
            }

            await _containerClient.UploadBlobAsync(fileName, fileStream, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<BlobContainerClient> AddNewContainer(string containerName, CancellationToken cancellationToken = default)
        {
            return await _blobServiceClient.CreateBlobContainerAsync(containerName, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task MoveBlobToArchive(string blobName, CancellationToken cancellationToken = default)
        {
            if (!await FileExistsAsync(blobName, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                throw new ArgumentException($"Blob {blobName} doesn't exist.", nameof(blobName));
            }

            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.SetAccessTierAsync(AccessTier.Archive, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Standard priority: The rehydration request will be processed in the order it was received and may take up to 15 hours.
        /// High priority: The rehydration request will be prioritized over Standard requests and may finish in under 1 hour for objects under ten GB in size.
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RehydrateBlob(string blobName, CancellationToken cancellationToken = default)
        {
            if (!await FileExistsAsync(blobName, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                throw new ArgumentException($"Blob {blobName} doesn't exist.", nameof(blobName));
            }

            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.SetAccessTierAsync(AccessTier.Hot, rehydratePriority: RehydratePriority.High, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
