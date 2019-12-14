using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Backend.Data
{
    public class BlobFileStorage : IFileStorage
    {
        private readonly CloudBlobContainer _blobContainer;

        public BlobFileStorage(CloudBlobContainer blobContainer)
        {
            _blobContainer = blobContainer;
        }

        public async Task<string> Add(string fileName, string filePath)
        {
            var blobRef = await GetBlobReference(fileName);
            await blobRef.UploadFromFileAsync(filePath);
            return blobRef.Uri.AbsoluteUri;
        }

        public async Task<string> Add(string fileName, MemoryStream fileData)
        {
            fileData.Position = 0;
            var blobRef = await GetBlobReference(fileName);
            await blobRef.UploadFromStreamAsync(fileData);
            return blobRef.Uri.AbsoluteUri;
        }

        private async Task<CloudBlockBlob> GetBlobReference(string fileName)
        {
            var blobReference = _blobContainer.GetBlockBlobReference(fileName);
            var exists = await blobReference.ExistsAsync();
            if (exists)
                throw new Exception("Blob already exists: " + fileName);

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (ext.EndsWith("jpg") || ext.EndsWith("jpeg"))
                blobReference.Properties.ContentType = "image/jpeg";
            else if(ext.EndsWith("png"))
                blobReference.Properties.ContentType = "image/png";

            return blobReference;
        }
    }
}
