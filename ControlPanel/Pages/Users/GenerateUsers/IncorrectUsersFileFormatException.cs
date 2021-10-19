using System;

namespace Olympiad.ControlPanel.Pages.Users.GenerateUsers
{
    public class IncorrectUsersFileFormatException : Exception
    {
        public string Summary { get; }
        public string Description { get; }
        public IncorrectUsersFileFormatException(string summary, string description) : base($"{summary}|{description}")
        {
            Summary = summary;
            Description = description;
        }
    }
}
