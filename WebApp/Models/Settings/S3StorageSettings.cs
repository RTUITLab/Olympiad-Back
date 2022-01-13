namespace WebApp.Models.Settings
{
    public class S3StorageSettings
    {
        public string ServiceUrl { get; set; }
        public bool ForcePathStyle { get; set; }
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string BucketName { get; set; }
    }
}
