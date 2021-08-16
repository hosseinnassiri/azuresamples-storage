using System;

namespace BlobStorageSample
{
    public sealed class ApplicationSettings
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Uri StorageAccountUrl { get; set; }
        public string Container { get; set; }
    }
}
