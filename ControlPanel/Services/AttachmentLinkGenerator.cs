using System;

namespace Olympiad.ControlPanel.Services
{
    public class AttachmentLinkGenerator
    {
        private readonly Uri baseApiAddress;

        public AttachmentLinkGenerator(Uri baseApiAddress)
        {
            this.baseApiAddress = baseApiAddress;
        }
        public string GetExerciseLink(Guid exerciseId, string fileName) => $"{baseApiAddress}api/exercises/{exerciseId}/attachment/{Uri.EscapeDataString(fileName)}";
    }
}
