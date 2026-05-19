namespace ProjectManagement.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string bucket, CancellationToken cancellationToken = default);
    Task<string> GetPresignedUrlAsync(string objectKey, string bucket, TimeSpan expiry, CancellationToken cancellationToken = default);
    Task DeleteAsync(string objectKey, string bucket, CancellationToken cancellationToken = default);
    Task EnsureBucketsExistAsync(CancellationToken cancellationToken = default);
}
