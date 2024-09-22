using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;

namespace DeliveryApp.Core.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;
        private readonly string _blobContainerName = "images";
        private readonly ILogger<BlobService> _logger;

        public BlobService(string connectionString, ILogger<BlobService> logger)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
            _logger = logger;
        }

        public async Task<string> AddBlob(string blobName, IEnumerable<byte> data)
        {
            try
            {
                var uniqueBlobName = GenerateUniqueBlobName(blobName);
                var blobClient = _containerClient.GetBlobClient(uniqueBlobName);
                using var memoryStream = new MemoryStream(data.ToArray());
                await blobClient.UploadAsync(memoryStream, true);
                return GenerateSasUri(blobClient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding blob {BlobName}", blobName);
                throw;
            }
        }

        private string GenerateUniqueBlobName(string blobName)
        {
            var extension = Path.GetExtension(blobName);
            var name = Path.GetFileNameWithoutExtension(blobName);
            return $"{name}_{Guid.NewGuid()}{extension}";
        }

        private string GenerateSasUri(BlobClient blobClient)
        {
            try
            {
                if (!blobClient.CanGenerateSasUri)
                {
                    _logger.LogWarning("BlobClient cannot generate SAS URI");
                    return blobClient.Uri.ToString();
                }

                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _blobContainerName,
                    BlobName = blobClient.Name,
                    ExpiresOn = DateTime.UtcNow.AddHours(1),
                    Resource = "b"
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                return blobClient.GenerateSasUri(sasBuilder).ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating SAS URI for blob {BlobName}", blobClient.Name);
                throw;
            }
        }

        public async Task<IEnumerable<byte>> GetBlob(string blobName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(blobName);
                using var stream = await blobClient.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving blob {BlobName}", blobName);
                throw;
            }
        }

        public string GetBlobUrl(string blobName)
        {
            try
            {
                return _containerClient.GetBlobClient(blobName).Uri.ToString();
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Request to Azure Blob Storage failed: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting the Blob URL: {ex.Message}");
                throw;
            }
        }
    }
}
