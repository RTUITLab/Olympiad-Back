using Amazon.S3;
using Amazon.S3.Model;
using ByteSizeLib;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models.Settings;

namespace WebApp.Services.Attachments;

public class S3AttachmentsService : IAttachmentsService
{
    private readonly AmazonS3Client s3Client;
    private readonly S3StorageSettings options;

    public S3AttachmentsService(AmazonS3Client s3Client, IOptions<S3StorageSettings> options)
    {
        this.s3Client = s3Client;
        this.options = options.Value;
    }

    

    public async Task<List<(string fileName, string contentType)>> GetAttachmentsForExercise(Guid exerciseId)
    {
        var url = await s3Client.ListObjectsV2Async(new ListObjectsV2Request
        {
            BucketName = options.BucketName,
            Prefix = ExercisesAttachmentsKey(exerciseId)
        });
        var targetObjects = url.S3Objects
            .Where(o => !string.IsNullOrEmpty(Path.GetFileName(o.Key)))
            .ToList();
        var metaDataRequests = targetObjects
            .Select(o => s3Client.GetObjectMetadataAsync(options.BucketName, o.Key))
            .ToList();
        await Task.WhenAll(metaDataRequests);
        return 
            targetObjects
            .Zip(metaDataRequests.Select(t => t.Result), (o, m) => (Path.GetFileName(o.Key), m.Headers.ContentType))
            .ToList();
    }

    public async Task DeleteExerciseAttachment(Guid exerciseId, string fileName)
    {
        await s3Client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = options.BucketName,
            Key = ExerciseAttachmentKey(exerciseId, fileName)
        });
    }

    public string GetUploadUrlForExercise(Guid exerciseId, string contentType, ByteSize uploadSize, string fileName)
    {
        var getUrlRequest = new GetPreSignedUrlRequest
        {
            BucketName = options.BucketName,
            Key = ExerciseAttachmentKey(exerciseId, fileName),
            Protocol = options.ServiceUrl.StartsWith("https") ? Protocol.HTTPS : Protocol.HTTP,
            Expires = DateTime.UtcNow.Add(TimeSpan.FromHours(1)),
            Verb = HttpVerb.PUT
        };
        // Set public access to object
        getUrlRequest.Headers["x-amz-acl"] = "public-read";
        getUrlRequest.Headers["Content-Length"] = ((long)uploadSize.Bytes).ToString();
        getUrlRequest.ContentType = contentType ?? "binary/octet-stream";

        return s3Client.GetPreSignedURL(getUrlRequest);
    }

    public string GetUrlForExerciseAttachment(Guid exerciseId, string fileName)
    {
        return $"{options.ServiceUrl}/{options.BucketName}/{ExercisesAttachmentsKey(exerciseId)}/{Uri.EscapeDataString(fileName)}";
    }
    private string ExercisesAttachmentsKey(Guid exerciseId) => $"exercises/{exerciseId}";
    private string ExerciseAttachmentKey(Guid exerciseId, string fileName) => $"{ExercisesAttachmentsKey(exerciseId)}/{fileName}";

    private string SolutionDocumentsKey(Guid solutionId) => $"solutions/{solutionId}/documents";
    private string SolutionDocumentKey(Guid solutionId, string fileName) => $"{SolutionDocumentsKey(solutionId)}/{fileName}";

    public async Task UploadSolutionDocument(Guid solutionId, string contentType, string fileName, Stream fileContent)
    {
        var response = await s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = options.BucketName,
            Key = SolutionDocumentKey(solutionId, fileName),
            ContentType = contentType,
            InputStream = fileContent,
            AutoCloseStream = true,
        });
        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception($"Can't upload file {fileName} for solution {solutionId}"); // TODO: soecific exception
        }
    }

    public async Task<(Stream fileStream, string contentType)> GetSolutionDocument(Guid solutionId, string fileName)
    {
        var response = await s3Client.GetObjectAsync(options.BucketName, SolutionDocumentKey(solutionId, fileName));
        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new ArgumentException($"Can't get file for solution {solutionId} and file {fileName}");
        }
        return (response.ResponseStream, response.Headers.ContentType);
    }
}
