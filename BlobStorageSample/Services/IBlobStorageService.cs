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
    }
}