using PublicAPI.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Olympiad.ControlPanel.Components.MarkdownEdit
{
    public interface IAttachmentsProvider
    {
        /// <summary>
        /// Unique key for attachments owner
        /// Used for disable update attachments for same provider
        /// </summary>
        string CurrentEditableEntityKey { get; }
        Task<List<AttachmentResponse>> GetAttachments();
        Task<string> GetUploadUrl(string fileName, long fileSize, string mimeType);
        string GetAttachmentUrl(string fileName);
        Task DeleteAttachment(string fileName);
    }
}
