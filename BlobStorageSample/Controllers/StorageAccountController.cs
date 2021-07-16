using Azure.Storage.Blobs;
using BlobStorageSample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlobStorageSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StorageAccountController : ControllerBase
    {
        private readonly BlobContainerClient _containerClient;
        private readonly IBlobStorageService _blobStorageService;

        public StorageAccountController(BlobContainerClient containerClient, IBlobStorageService blobStorageService)
        {
            _containerClient = containerClient;
            _blobStorageService = blobStorageService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<string>>> Get()
        {
            return Ok(await _blobStorageService.GetAllFileNamesAsync().ConfigureAwait(false));
        }

        [HttpGet("{fileName}")]
        public async Task<ActionResult> Download(string fileName)
        {
            if (!await _blobStorageService.FileExistsAsync(fileName).ConfigureAwait(false))
            {
                return NotFound();
            }

            return File(await _blobStorageService.DownloadAsync(fileName).ConfigureAwait(false), "application/octet-stream", fileName);
        }

        [HttpPost]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Upload([FromForm] IFormFile file)
        {
            if (file.Length <= 0)
            {
                return NoContent();
            }

            await _blobStorageService.UploadAsync(file.FileName, file.OpenReadStream()).ConfigureAwait(false);

            return Ok();
        }
    }
}
