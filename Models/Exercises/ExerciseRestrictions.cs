using ByteSizeLib;
using Olympiad.Shared;
using System;
using System.Collections.Generic;

namespace Models.Exercises
{
    public sealed class ExerciseRestrictions
    {
        public CodeRestrictions Code { get; set; }
        public DocsRestrictions Docs { get; set; }
    }
    public sealed class CodeRestrictions
    {
        public List<string> AllowedRuntimes { get; set; }
    }

    public sealed class DocsRestrictions
    {
        public List<DocumentRestriction> Documents { get; set; }
    }
    public sealed class DocumentRestriction
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
