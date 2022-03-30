using Ardalis.SmartEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympiad.Shared
{
    public class ProgramRuntime : SmartEnum<ProgramRuntime, string>
    {
        public static ProgramRuntime Java = new ProgramRuntime("Java", "java", ".java");
        public static ProgramRuntime CSharp = new ProgramRuntime("C#", "csharp", ".cs");
        public static ProgramRuntime PasAbc = new ProgramRuntime("Pascal ABC", "pasabc", ".pas", "pascal");
        public static ProgramRuntime FreePas = new ProgramRuntime("Free pascal", "fpas", ".pas", "pascal");
        public static ProgramRuntime C = new ProgramRuntime("C", "c", ".c");
        public static ProgramRuntime Cpp = new ProgramRuntime("C++", "cpp", ".cpp");
        public static ProgramRuntime Js = new ProgramRuntime("JS", "js", ".js");
        public static ProgramRuntime Python = new ProgramRuntime("Python", "python", ".py");

        /// <summary>
        /// File extension for solution file
        /// </summary>
        public string FileExtension { get; }
        /// <summary>
        /// Lang token for prism.js
        /// </summary>
        public string PrismLang { get; }
        private ProgramRuntime(string name, string value, string extension, string prismLang = null) : base(name, value)
        {
            FileExtension = extension;
            PrismLang = prismLang ?? value;
        }
        protected ProgramRuntime() : this("unknown", "unknown", ".txt") { }
        public static string GetFileExtensionForRuntime(string runtime)
            => TryFromValue(runtime, out var found) ? found.FileExtension : ".txt";
    }
}
