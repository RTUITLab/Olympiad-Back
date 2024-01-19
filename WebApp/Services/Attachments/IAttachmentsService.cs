using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebApp.Services.Attachments;

public interface IAttachmentsService
{
    Task<List<(string fileName, string contentType)>> GetAttachmentsForExercise(Guid exerciseId);
    Task UploadExerciseAttachment(Guid exerciseId, string contentType, string fileName, Stream fileContent);
    Task<(Stream fileStream, string contentType)> GetExerciseAttachment(Guid solutionId, string fileName);
    Task DeleteExerciseAttachment(Guid exerciseId, string fileName);


    Task UploadSolutionDocument(Guid solutionId, string contentType, string fileName, Stream fileContent);
    Task<(Stream fileStream, string contentType)> GetSolutionDocument(Guid solutionId, string fileName);
}
