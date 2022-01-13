using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Services.Attachments
{
    public interface IAttachmentsService
    {
        Task<List<string>> GetAttachmentsForExercise(Guid exerciseId);
        string GetUploadUrlForExercise(Guid exerciseId, string contentType, ByteSize uploadSize, string fileName);
        string GetUrlForExerciseAttachment(Guid exerciseId, string fileName);
    }
}
