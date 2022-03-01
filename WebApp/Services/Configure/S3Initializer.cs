using Amazon.S3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Models.Settings;

namespace WebApp.Services.Configure
{
    public class S3Initializer : IConfigureWork
    {
        private readonly AmazonS3Client s3Client;
        private readonly IOptions<S3StorageSettings> options;
        private readonly ILogger<S3Initializer> logger;

        public S3Initializer(AmazonS3Client s3Client, IOptions<S3StorageSettings> options, ILogger<S3Initializer> logger)
        {
            this.s3Client = s3Client;
            this.options = options;
            this.logger = logger;
        }
        public async Task Configure(CancellationToken cancellationToken)
        {
            var delayBetweenTry = TimeSpan.FromSeconds(5);
            foreach (var tryNum in Enumerable.Range(1, 5))
            {
                logger.LogInformation("Initializing S3 try #{TryNum} ServiceUrl: '{ServiceUrl}' ...", tryNum, options.Value.ServiceUrl);
                try
                {
                    using var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    cancellationSource.CancelAfter(TimeSpan.FromSeconds(5));
                    await InitializeS3(options.Value, cancellationSource.Token);
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Can't initialize S3, waiting {Delay} to next try", delayBetweenTry);
                    await Task.Delay(delayBetweenTry, cancellationToken);
                }
            }
            throw new Exception("Can't initialize S3, check options and environment");
        }

        private async Task InitializeS3(S3StorageSettings options, CancellationToken cancellationToken)
        {
            try
            {
                var location = await s3Client.GetBucketLocationAsync(options.BucketName);
                if (location.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Status code of location request is not OK: {location.HttpStatusCode}");
                }
                logger.LogInformation("Bucket {Bucket} already created", options.BucketName);
                return;
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, "Bucket {Bucket} not found", options.BucketName);
            }
            logger.LogInformation("Creating bucket {Bucket}", options.BucketName);
            var result = await s3Client.PutBucketAsync(options.BucketName, cancellationToken);
            logger.LogInformation("S3 storage initialized with bucket {BucketName} StatusCode: {StatusCode}", options.BucketName, result.HttpStatusCode);
        }
    }
}
