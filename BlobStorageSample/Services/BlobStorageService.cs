using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BlobStorageSample.Services
{
    public sealed class BlobStorageService(BlobContainerClient containerClient, BlobServiceClient blobServiceClient, ILogger<BlobStorageService> logger) : IBlobStorageService
    {
		private readonly ILogger _logger = logger;

		public async Task<IReadOnlyList<BlobItem>> GetAllBlobsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Listing all blobs in container {@container} in storage account {@storageAccount}.", containerClient.Name, containerClient.AccountName);
            var blobs = containerClient.GetBlobsAsync(cancellationToken: cancellationToken);
            var list = new List<BlobItem>();
            await foreach (BlobItem blob in blobs)
            {
                list.Add(blob);
            }
            return list;
        }

        public async Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken = default)
        {
            return await containerClient.GetBlobClient(fileName).ExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<byte[]> DownloadAsync(string fileName, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Downloading blob {@blobName}.", fileName);
            if (!await FileExistsAsync(fileName, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                _logger.LogError("Cannot download {@blobName}, it doesn't exist.", fileName);
                throw new ArgumentException($"Blob {fileName} doesn't exist.", nameof(fileName));
            }
            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

            return stream.ToArray();
        }

        public async Task<Stream> DownloadAsStreamAsync(string fileName, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Downloading blob {@blobName}.", fileName);
            if (!await FileExistsAsync(fileName, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                _logger.LogError("Cannot download {@blobName}, it doesn't exist.", fileName);
                throw new ArgumentException($"Blob {fileName} doesn't exist.", nameof(fileName));
            }
            var blobClient = containerClient.GetBlobClient(fileName);

            return await blobClient.OpenReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UploadAsync(string fileName, Stream fileStream, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Uploading blob {@blobName}.", fileName);
            if (await FileExistsAsync(fileName, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                _logger.LogInformation("Blob {@blobName} already existis, trying to overwrite.", fileName);
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(fileStream, overwrite: true, cancellationToken: cancellationToken).ConfigureAwait(false);
                return true;
            }

            await containerClient.UploadBlobAsync(fileName, fileStream, cancellationToken: cancellationToken).ConfigureAwait(false);
            return true;
        }

        public async Task<BlobContainerClient> AddNewContainerAsync(string containerName, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding new container {@container} to storage account.", containerName);
            return await blobServiceClient.CreateBlobContainerAsync(containerName, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> MoveBlobToArchiveAsync(string blobName, CancellationToken cancellationToken = default)
        {
            if (!await FileExistsAsync(blobName, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                _logger.LogError("Cannot archive {@blobName}, it doesn't exist.", blobName);
                throw new ArgumentException($"Blob {blobName} doesn't exist.", nameof(blobName));
            }

            var blobClient = containerClient.GetBlobClient(blobName);
            var result = await blobClient.SetAccessTierAsync(AccessTier.Archive, cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Archiving process has started for blob {@blobName}.", blobName);
            return result.Status.Equals((int)HttpStatusCode.OK);
        }

        public async Task<bool> RehydrateBlobAsync(string blobName, RehydratePriority priority = RehydratePriority.Standard, CancellationToken cancellationToken = default)
        {
            if (!await FileExistsAsync(blobName, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                _logger.LogError("Cannot rehydrate {@blobName}, it doesn't exist.", blobName);
                throw new ArgumentException($"Blob {blobName} doesn't exist.", nameof(blobName));
            }

            var blobClient = containerClient.GetBlobClient(blobName);
            var result = await blobClient.SetAccessTierAsync(AccessTier.Hot, rehydratePriority: priority, cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Rehydrating process has started for blob {@blobName} with priority {@priority}. it can take up to 1 or 15 hours depending on the priority.", blobName, priority);
            return result.Status.Equals((int)HttpStatusCode.OK);
        }
    }
}
