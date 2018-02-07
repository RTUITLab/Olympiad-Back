using System;
using System.Collections.Generic;
using System.Text;

namespace Executor.Executers
{
    [AttributeUsage(System.AttributeTargets.Class)]
    class LanguageAttribute : Attribute
    {
        public string Lang { get; }
        public LanguageAttribute(string lang)
        {
            Lang = lang;
        }

    }
}
