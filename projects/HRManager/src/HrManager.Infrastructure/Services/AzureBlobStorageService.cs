using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.Common.Services;
using HrManager.Domain.Enums;

namespace HrManager.Infrastructure.Services;

public class AzureBlobStorageService(
    BlobContainerClient _containerClient,
    IDateTimeService _dateTime) : IStorageService
{
    public string GenerateBlobName(Guid employeeId, DocumentType documentType, string fileName)
    {
        return $"{employeeId}/{documentType}/{Path.GetFileNameWithoutExtension(fileName)}_{_dateTime.UtcNow:yyyy.MM.dd.HH.mm.ss}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
    }

    public async Task<bool> DeleteFileAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        var response = await blobClient.DeleteIfExistsAsync();
        return response.Value;
    }

    public async Task<Uri> GetSasUriAsync(string blobName, TimeSpan expiry)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            throw new NotFoundException("Document has been deleted");
        }

        if (!blobClient.CanGenerateSasUri)
        {
            throw new InvalidOperationException("SAS URI generation not supported.");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerClient.Name,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiry),
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
        return await Task.FromResult(sasUri);
    }

    public async Task<string> UploadFileAsync(string blobName, Stream content)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(content, overwrite: true);
        return blobClient.Uri.ToString();
    }
}
