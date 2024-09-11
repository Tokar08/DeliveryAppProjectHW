using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace DeliveryApp.Core.Services;

public class BlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _containerClient;
    private readonly string _blobContainerName = "images";

    public BlobService(string connectionString)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task<(string name, string sas)> AddBlob(string blobName, IEnumerable<byte> data)
    {
        string uniqueName = $"{blobName}{Guid.NewGuid()}";
        var blob = _containerClient.GetBlobClient(uniqueName);
        using var ms = new MemoryStream(data.ToArray());
        var sas = GenerateSAS(blob);
        await blob.UploadAsync(ms);
        return (uniqueName, sas);
    }

    private string GenerateSAS(BlobClient blob)
    {
        if(!blob.CanGenerateSasUri) {
            return string.Empty;
        }

        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = _blobContainerName,
            BlobName = blob.Name,
            ExpiresOn = DateTime.UtcNow.AddMinutes(30),
            Resource = "b"
        };
        sasBuilder.SetPermissions(BlobAccountSasPermissions.All);
        return blob.GenerateSasUri(sasBuilder).ToString();
    }

    public async Task<IEnumerable<byte>> GetBlob(string blobName)
    {
        var blob = _containerClient.GetBlobClient(blobName);
        using var stream = await blob.OpenReadAsync();
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
