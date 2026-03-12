using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using HrManager.Application.Common.Exceptions;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.Common.Services;
using HrManager.Domain.Enums;
using HrManager.Infrastructure.Persistance.Configurations.Settings;

namespace HrManager.Infrastructure.Services;

public class FirebaseStorageService : IStorageService
{
    private readonly StorageClient storageClient;
    private readonly UrlSigner urlSigner;
    private readonly string bucketName;
    private readonly IDateTimeService dateTime;

    public FirebaseStorageService(FirebaseStorageSettings settings, IDateTimeService dateTimeService)
    {
        dateTime = dateTimeService;
        bucketName = settings.BucketName;

        ServiceAccountCredential serviceAccountCredential;

        if (!string.IsNullOrEmpty(settings.ServiceAccountKeyJson))
        {
            using var jsonStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(settings.ServiceAccountKeyJson));
            serviceAccountCredential = ServiceAccountCredential.FromServiceAccountData(jsonStream);
        }
        else
        {
            using var keyStream = File.OpenRead(settings.ServiceAccountKeyPath);
            serviceAccountCredential = ServiceAccountCredential.FromServiceAccountData(keyStream);
        }

        storageClient = StorageClient.Create(serviceAccountCredential.ToGoogleCredential());
        urlSigner = UrlSigner.FromCredential(serviceAccountCredential);
    }

    public string GenerateBlobName(Guid employeeId, DocumentType documentType, string fileName)
    {
        return $"{employeeId}/{documentType}/{Path.GetFileNameWithoutExtension(fileName)}_{dateTime.UtcNow:yyyy.MM.dd.HH.mm.ss}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
    }

    public async Task<string> UploadFileAsync(string blobName, Stream content)
    {
        await storageClient.UploadObjectAsync(bucketName, blobName, null, content);
        return $"https://storage.googleapis.com/{bucketName}/{Uri.EscapeDataString(blobName)}";
    }

    public async Task<Uri> GetSasUriAsync(string blobName, TimeSpan expiry)
    {
        try
        {
            await storageClient.GetObjectAsync(bucketName, blobName);
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new NotFoundException("Document has been deleted");
        }

        string signedUrl = await urlSigner.SignAsync(
            bucketName,
            blobName,
            expiry,
            HttpMethod.Get);

        return new Uri(signedUrl);
    }

    public async Task<bool> DeleteFileAsync(string blobName)
    {
        try
        {
            await storageClient.DeleteObjectAsync(bucketName, blobName);
            return true;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
