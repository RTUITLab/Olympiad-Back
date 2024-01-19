using PublicAPI.Responses;
using System.Collections.Generic;
using System.IO;
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
        Task UploadAttachment(string fileName, long fileSize, string mimeType, Stream content);
        string GetAttachmentUrl(string fileName);
        Task DeleteAttachment(string fileName);
    }
}
