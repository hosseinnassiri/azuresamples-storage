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
        Task<IReadOnlyList<string>> GetAllFileNamesAsync(CancellationToken cancellationToken = default);
        Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken = default);
        Task<byte[]> DownloadAsync(string fileName, CancellationToken cancellationToken = default);
        Task UploadAsync(string fileName, Stream fileStream, CancellationToken cancellationToken = default);
        Task<BlobContainerClient> AddNewContainer(string containerName, CancellationToken cancellationToken = default);
        Task MoveBlobToArchive(string blobName, CancellationToken cancellationToken = default);
        Task RehydrateBlob(string blobName, RehydratePriority priority = RehydratePriority.Standard, CancellationToken cancellationToken = default);
    }
}