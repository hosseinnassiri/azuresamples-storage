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
            return Ok(await _blobStorageService.GetAllFileNamesAsync(cancellationToken: cancellationToken).ConfigureAwait(false));
        }

        [HttpGet("{fileName}")]
        public async Task<ActionResult> Download(string fileName, CancellationToken cancellationToken)
        {
            if (!await _blobStorageService.FileExistsAsync(fileName, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                return NotFound();
            }

            return File(await _blobStorageService.DownloadAsync(fileName, cancellationToken: cancellationToken).ConfigureAwait(false), "application/octet-stream", fileName);
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
    }
}
