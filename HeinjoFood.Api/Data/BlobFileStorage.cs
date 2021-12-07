using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HeinjoFood.Api.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

// https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet

namespace HeinjoFood.Api.Data
{
    public class BlobFileStorage : IFileStorage
    {
        private readonly BlobContainerClient _blobContainer;
        private readonly StorageOptions _options;
        private readonly ILogger<BlobFileStorage> _logger;

        public BlobFileStorage(IOptions<StorageOptions> options, ILogger<BlobFileStorage> logger)
        {
            _options = options.Value;
            _logger = logger;
            var blobServiceClient = new BlobServiceClient(_options.StorageAccount);
            _blobContainer = blobServiceClient.GetBlobContainerClient("img");
        }


        public async Task<Stream?> GetAsync(string fileName)
        {
            var blob = _blobContainer.GetBlobClient(fileName);
            if (await blob.ExistsAsync())
            {
                _logger.LogInformation("Download streaming of file {FileName}", fileName);
                var streaming = await blob.DownloadStreamingAsync();
                return streaming.Value.Content;
            }
            return null;
        }

        public async Task<string> Add(string fileName, string filePath)
        {
            var blob = await GetBlobClient(fileName);
            await blob.UploadAsync(filePath, new BlobHttpHeaders { ContentType = GetContentType(fileName) });
            _logger.LogInformation("Uploaded new file {BlobUri}", blob.Uri.AbsoluteUri);
            return blob.Uri.AbsoluteUri;
        }

        public async Task<string> Add(string fileName, MemoryStream content)
        {
            content.Position = 0;
            var blob = await GetBlobClient(fileName);
            await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = GetContentType(fileName) });
            _logger.LogInformation("Uploaded new file {BlobUri}", blob.Uri.AbsoluteUri);
            return blob.Uri.AbsoluteUri;
        }

        private async Task<BlobClient> GetBlobClient(string fileName)
        {
            // Get a reference to a blob
            var blob = _blobContainer.GetBlobClient(fileName);
            var exists = await blob.ExistsAsync();
            if (exists)
                throw new Exception("Blob already exists: " + fileName);
            return blob;
        }

        public string? GetContentType(string fileName)
        {
            if (new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType))
                return contentType;
            else
                return null;
        }

    }
}
