using System;

namespace Olympiad.ControlPanel.Pages.Users.GenerateUsers
{
    public class IncorrectColumnsException : Exception
    {
        public string Summary { get; }
        public string Description { get; }
        public IncorrectColumnsException(string summary, string description) : base($"{summary}|{description}")
        {
            Summary = summary;
            Description = description;
        }
    }
}
