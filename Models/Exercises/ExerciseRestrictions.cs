using ByteSizeLib;
using Olympiad.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Exercises
{
    public class ExerciseRestrictions
    {
        public CodeRestrictions Code { get; set; }
        public DocsRestrictions Docs { get; set; }
    }
    public class CodeRestrictions
    {
        public List<string> AllowedRuntimes { get; set; }
    }

    public class DocsRestrictions
    {
        public List<DocumentRestriction> Documents { get; set; }
    }
    public class DocumentRestriction
    {
        public List<string> AllowedExtensions { get; set; }
        /// <summary>
        /// Max size in bytes
        /// </summary>
        public double MaxSize { get; set; }
        /// <summary>
        /// Document title for user
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Document description for user
        /// </summary>
        public string Description { get; set; }
    }
}
