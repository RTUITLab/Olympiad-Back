using Docker.DotNet.Models;
using Executor.Models.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;

namespace Executor;

static class Extensions
{
    public static ConsoleMode GetConsoleMode(this IConfiguration configuration)
    {
        return configuration.GetValue<ConsoleMode>("CONSOLE_MODE");
    }


    public static IServiceCollection ConfigureAndValidate<T>(
        this IServiceCollection services,
        IConfigurationSection config) where T : class
    {
        services.Configure<T>(config);
        services.PostConfigure<T>(settings =>
         {
             var configErrors = ValidationErrors(settings).ToArray();
             if (configErrors.Any())
             {
                 var aggrErrors = string.Join(",", configErrors);
                 var count = configErrors.Length;
                 var configType = typeof(T).Name;
                 throw new ApplicationException(
                     $"Found {count} configuration error(s) in {configType}: {aggrErrors}");
             }
         });
        return services;
    }

    private static IEnumerable<string> ValidationErrors<T>(T obj)
    {
        var context = new ValidationContext(obj, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(obj, context, results, true);
        foreach (var validationResult in results)
        {
            yield return validationResult.ErrorMessage;
        }
    }
    private static readonly JsonSerializerOptions skipNullOptions = new JsonSerializerOptions()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
    };

    public static string LogsToString(this IEnumerable<JSONMessage> messages)
    {
        return string.Join('\n', messages.Select(m => JsonSerializer.Serialize(m, skipNullOptions)));
    }

}
