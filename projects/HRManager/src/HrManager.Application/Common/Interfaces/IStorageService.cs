using HrManager.Domain.Enums;

namespace HrManager.Application.Common.Interfaces;

public interface IStorageService
{
    string GenerateBlobName(Guid employeeId, DocumentType documentType, string fileName);

    Task<string> UploadFileAsync(string blobName, Stream content);

    Task<Uri> GetSasUriAsync(string blobName, TimeSpan expiry);

    Task<bool> DeleteFileAsync(string blobName);
}
