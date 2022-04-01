using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Olympiad.Shared.Models;

namespace Olympiad.Shared.JsonConverters
{
    public static class JsonConvertersHelper
    {
        public static void AddCustomConverters(this IList<JsonConverter> converters)
        {
            converters.Add(new DateTimeConverter());
            converters.Add(new TimeSpanConverter());
            converters.Add(new ByteSizeConverter());
            converters.Add(new SmartEnumValueConverter<ProgramRuntime, string>());
            converters.Add(new SmartEnumValueConverter<ExerciseType, int>());
        }
    }
}
