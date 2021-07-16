using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BlobStorageSample.Services
{
    public sealed class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(BlobContainerClient containerClient)
        {
            _containerClient = containerClient;
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
    }
}
