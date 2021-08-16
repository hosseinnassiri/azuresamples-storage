using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BlobStorageSample.Services
{
    public interface IBlobStorageService
    {
        Task<IReadOnlyList<BlobItem>> GetAllBlobsAsync(CancellationToken cancellationToken = default);
        Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken = default);
        Task<byte[]> DownloadAsync(string fileName, CancellationToken cancellationToken = default);
        Task<Stream> DownloadAsStreamAsync(string fileName, CancellationToken cancellationToken = default);
        Task UploadAsync(string fileName, Stream fileStream, CancellationToken cancellationToken = default);
        Task<BlobContainerClient> AddNewContainerAsync(string containerName, CancellationToken cancellationToken = default);
        Task MoveBlobToArchiveAsync(string blobName, CancellationToken cancellationToken = default);
        Task RehydrateBlobAsync(string blobName, RehydratePriority priority = RehydratePriority.Standard, CancellationToken cancellationToken = default);
    }
}