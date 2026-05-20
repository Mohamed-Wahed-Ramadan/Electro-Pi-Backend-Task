using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Infrastructure.Options;

namespace ProjectManagement.Infrastructure.Services;

public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _client;
    private readonly MinioSettings _settings;
    private readonly ILogger<MinioFileStorageService> _logger;
    private static readonly string[] Buckets = ["project-covers", "task-attachments"];

    public MinioFileStorageService(IMinioClient client, IOptions<MinioSettings> settings, ILogger<MinioFileStorageService> logger)
    {
        _client = client;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task EnsureBucketsExistAsync(CancellationToken cancellationToken = default)
    {
        foreach (var bucket in Buckets)
        {
            var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket), cancellationToken);
            if (!exists)
            {
                await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket), cancellationToken);
                _logger.LogInformation("Created MinIO bucket: {Bucket}", bucket);
            }
        }
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string bucket, CancellationToken cancellationToken = default)
    {
        await EnsureBucketsExistAsync(cancellationToken);

        var putArgs = new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);

        await _client.PutObjectAsync(putArgs, cancellationToken);
        return await GetPresignedUrlAsync(fileName, bucket, TimeSpan.FromDays(7), cancellationToken);
    }

    public async Task<string> GetPresignedUrlAsync(string objectKey, string bucket, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectKey)
            .WithExpiry((int)expiry.TotalSeconds);

        var url = await _client.PresignedGetObjectAsync(args);
        return RewriteToPublicUrl(url);
    }

    public async Task DeleteAsync(string objectKey, string bucket, CancellationToken cancellationToken = default)
    {
        await _client.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(bucket).WithObject(objectKey), cancellationToken);
    }

    private string RewriteToPublicUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(_settings.PublicUrl))
            return url;

        try
        {
            var source = new Uri(url);
            var target = new Uri(_settings.PublicUrl);

            var builder = new UriBuilder(source)
            {
                Scheme = target.Scheme,
                Host = target.Host,
                Port = target.IsDefaultPort ? -1 : target.Port
            };

            return builder.Uri.ToString();
        }
        catch
        {
            return url;
        }
    }
}
