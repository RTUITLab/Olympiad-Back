using Amazon.S3;
using Amazon.S3.Model;
using ByteSizeLib;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApp.Models.Settings;

namespace WebApp.Services.Attachments
{
    public class S3AttachmentsService : IAttachmentsService
    {
        private readonly AmazonS3Client s3Client;
        private readonly S3StorageSettings options;

        public S3AttachmentsService(AmazonS3Client s3Client, IOptions<S3StorageSettings> options)
        {
            this.s3Client = s3Client;
            this.options = options.Value;
        }
        public async Task<List<string>> GetAttachmentsForExercise(Guid exerciseId)
        {
            var url = await s3Client.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName = options.BucketName,
                Prefix = ExerciseAttachmentsKey(exerciseId)
            });
            return url.S3Objects
                .Select(o => Path.GetFileName(o.Key))
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();
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
            getUrlRequest.ContentType = contentType;

            return s3Client.GetPreSignedURL(getUrlRequest);
        }

        public string GetUrlForExerciseAttachment(Guid exerciseId, string fileName)
        {
            return $"{options.ServiceUrl}/{options.BucketName}/{ExerciseAttachmentsKey(exerciseId)}/{Uri.EscapeDataString(fileName)}";
        }
        private string ExerciseAttachmentKey(Guid exerciseId, string fileName) => $"{ExerciseAttachmentsKey(exerciseId)}/{fileName}";
        private string ExerciseAttachmentsKey(Guid exerciseId) => $"exercises/{exerciseId}";
    }
}
