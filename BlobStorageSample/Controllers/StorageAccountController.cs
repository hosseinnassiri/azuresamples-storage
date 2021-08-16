using Azure.Storage.Blobs.Models;
using BlobStorageSample.Requests;
using BlobStorageSample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlobStorageSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StorageAccountController : ControllerBase
    {
        private readonly IBlobStorageService _blobStorageService;

        public StorageAccountController(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<string>>> Get(CancellationToken cancellationToken)
        {
            return Ok(await _blobStorageService.GetAllBlobsAsync(cancellationToken: cancellationToken).ConfigureAwait(false));
        }

        [HttpGet("{fileName}")]
        public async Task<ActionResult> Download(string fileName, CancellationToken cancellationToken)
        {
            if (!await _blobStorageService.FileExistsAsync(fileName, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            return File(await _blobStorageService.DownloadAsStreamAsync(fileName, cancellationToken: cancellationToken).ConfigureAwait(false), "application/octet-stream", fileName);
        }

        [HttpPost]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Upload([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            if (file.Length <= 0)
            {
                return NoContent();
            }

            await _blobStorageService.UploadAsync(file.FileName, file.OpenReadStream(), cancellationToken: cancellationToken).ConfigureAwait(false);

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> AddContainer([FromBody] AddNewContainer request, CancellationToken cancellationToken)
        {
            await _blobStorageService.AddNewContainerAsync(request.ContainerName, cancellationToken: cancellationToken).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Archive([FromBody] ArchiveFile request, CancellationToken cancellationToken)
        {
            await _blobStorageService.MoveBlobToArchiveAsync(request.BlobName, cancellationToken: cancellationToken).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Rehydrate([FromBody] RehydrateFile request, CancellationToken cancellationToken)
        {
            await _blobStorageService.RehydrateBlobAsync(request.BlobName, priority: RehydratePriority.High, cancellationToken: cancellationToken).ConfigureAwait(false);
            return Ok();
        }
    }
}
