using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BlobStorageSample.Services
{
    public interface IBlobStorageService
    {
        Task<IReadOnlyList<string>> GetAllFileNamesAsync();
        Task<bool> FileExistsAsync(string fileName);
        Task<byte[]> DownloadAsync(string fileName);
        Task UploadAsync(string fileName, Stream fileStream);
    }
}