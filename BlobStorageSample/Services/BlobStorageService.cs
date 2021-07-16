using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using System.IO;
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

        public async Task<IReadOnlyList<string>> GetAllFileNamesAsync()
        {
            var blobs = _containerClient.GetBlobsAsync();
            var list = new List<string>();
            await foreach (BlobItem blob in blobs)
            {
                list.Add(blob.Name);
            }
            return list;
        }

        public async Task<bool> FileExistsAsync(string fileName)
        {
            return await _containerClient.GetBlobClient(fileName).ExistsAsync().ConfigureAwait(false);
        }

        public async Task<byte[]> DownloadAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);

            using var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream).ConfigureAwait(false);

            return stream.ToArray();
        }

        public async Task UploadAsync(string fileName, Stream fileStream)
        {
            if (await FileExistsAsync(fileName).ConfigureAwait(false))
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(fileStream).ConfigureAwait(false);
            }
            await _containerClient.UploadBlobAsync(fileName, fileStream).ConfigureAwait(false);
        }
    }
}
