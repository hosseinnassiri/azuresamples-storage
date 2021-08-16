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
        /// <summary>
        /// Retrieves list of all blobs in the container of the storage account.
        /// </summary>
        /// <param name="cancellationToken">Optional System.Threading.CancellationToken to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns a list of Azure Storage blobs.</returns>
        Task<IReadOnlyList<BlobItem>> GetAllBlobsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the blob exists in the container on the storage account.
        /// </summary>
        /// <param name="fileName">The name of the blob to check.</param>
        /// <param name="cancellationToken">Optional System.Threading.CancellationToken to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns true if the blob exists.</returns>
        Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Downloads a blob using parallel requests, and writes the content to a byte array.
        /// </summary>
        /// <param name="fileName">The name of the blob to download.</param>
        /// <param name="cancellationToken">Optional System.Threading.CancellationToken to propagate notifications that the operation should be cancelled.</param>
        /// <returns>A byte array containing the content of the blob.</returns>
        Task<byte[]> DownloadAsync(string fileName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Downloads the file by opening a stream for reading from the blob.
        /// </summary>
        /// <param name="fileName">The name of the blob to download.</param>
        /// <param name="cancellationToken">Optional System.Threading.CancellationToken to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns a stream that will download the blob.</returns>
        Task<Stream> DownloadAsStreamAsync(string fileName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uploads the file from stream to blob storage.
        /// </summary>
        /// <param name="fileName">The name of the blob to upload.</param>
        /// <param name="fileStream">The stream containing the file content to upload.</param>
        /// <param name="cancellationToken">Optional System.Threading.CancellationToken to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns true if upload is successful.</returns>
        Task<bool> UploadAsync(string fileName, Stream fileStream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new blob container under the storage account. If the container with the same name already exists, the operation fails.
        /// </summary>
        /// <param name="containerName">The name of the container to create.</param>
        /// <param name="cancellationToken">Optional System.Threading.CancellationToken to propagate notifications that the operation should be cancelled.</param>
        /// <returns>The Azure.Storage.Blobs.BlobContainerClient for the newly created container which allows you to manipulate Azure Storage container and its blobs.</returns>
        Task<BlobContainerClient> AddNewContainerAsync(string containerName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Moves a blob to archive access tier.
        /// </summary>
        /// <param name="blobName">The name of the blob to archive.</param>
        /// <param name="cancellationToken">Optional System.Threading.CancellationToken to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns true if the archiving action is successful.</returns>
        Task<bool> MoveBlobToArchiveAsync(string blobName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Rehydrates an archived blob to make it accessible again.
        /// Standard priority: The rehydration request will be processed in the order it was received and may take up to 15 hours.
        /// High priority: The rehydration request will be prioritized over Standard requests and may finish in under 1 hour for objects under ten GB in size.
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-rehydration?tabs=azure-portal#rehydrate-an-archived-blob-to-an-online-tier"/>
        /// </summary>
        /// <param name="blobName">The name of the archived blob to rehydrate.</param>
        /// <param name="cancellationToken">Optional System.Threading.CancellationToken to propagate notifications that the operation should be cancelled.</param>
        /// <returns>Returns true if the rehydrating action is successful.</returns>
        Task<bool> RehydrateBlobAsync(string blobName, RehydratePriority priority = RehydratePriority.Standard, CancellationToken cancellationToken = default);
    }
}