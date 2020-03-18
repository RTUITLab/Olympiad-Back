using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using PublicAPI.Responses.Solutions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Executor
{
    class ConsoleStatusReporter
    {
        private static string[] twentyPercentFillings = new string[]
        {
            "[>....]",
            "[=>...]",
            "[==>..]",
            "[===>.]",
            "[====>]",
            "[=====]"
        };
        private List<SolutionsStatisticResponse> statistic;
        private readonly ISolutionsBase solutionsBase;
        private readonly ILogger<ConsoleStatusReporter> logger;
        private DateTime lastClear;
        public ConsoleStatusReporter(ISolutionsBase solutionsBase, ILogger<ConsoleStatusReporter> logger)
        {
            this.solutionsBase = solutionsBase;
            this.logger = logger;
        }

        internal async Task Start(
            Executor executor,
            CancellationToken cancellationToken)
        {
            Console.Clear();
            lastClear = DateTime.Now;
            var statisticTask = PingStatisticTask(cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (DateTime.Now - lastClear > TimeSpan.FromSeconds(10))
                    {
                        Console.Clear();
                        lastClear = DateTime.Now;
                    }
                    Renderpage(executor, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"error while status printing");
                }
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
            }
            await statisticTask;
        }

        private void Renderpage(Executor executor, CancellationToken cancellationToken)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"BUILD: {executor.BuildQueueLength}");
            foreach (var worker in executor.executeWorkers)
            {
                var current = worker.Value.builder.Current?.Id;
                var currentMessage = current == null ? "null" :
                    $"{current} {worker.Value.builder.CurrentBuildTime:hh\\:mm\\:ss}";
                Console.WriteLine($"{worker.Key,6}: {worker.Value.builder.BuildQueueLength,3} | current: {currentMessage,45}");
            }

            Console.WriteLine($"RUN: {executor.RunQueueLength}");
            foreach (var worker in executor.executeWorkers)
            {
                var runner = worker.Value.runner;
                var current = runner.Current;
                string currentMessage;
                if (current == null)
                {
                    currentMessage = "null";
                }
                else
                {
                    string percentPart;
                    if (runner.CurrentTestDataCount == 0)
                    {
                        percentPart = "[nodat]";
                    }
                    else
                    {
                        var twentyPartLength = runner.CurrentTestDataCount / 5d;
                        var twentyPercentPartsCount = (int)(runner.CurrentTestDataCheckedCount / twentyPartLength);
                        percentPart = twentyPercentFillings[twentyPercentPartsCount];
                    }
                    currentMessage = current == null ? "null" :
                        $"{current} ({runner.CurrentTestDataCheckedCount,4}/{runner.CurrentTestDataCount,4}) {percentPart} {runner.CurrentBuildTime:hh\\:mm\\:ss}";

                }
                Console.WriteLine($"{worker.Key,6}: {runner.RunQueueLength,3} | current: {currentMessage,65}");
            }
            string inQueueOnServerMessage;
            var inQueueOnServerObject = statistic?.FirstOrDefault(o => o.SolutionStatus == "InQueue");
            if (inQueueOnServerObject == null)
            {
                inQueueOnServerMessage = "no data";
            }
            else
            {
                inQueueOnServerMessage = $"{inQueueOnServerObject.Count,7}";
            }
            Console.WriteLine($"In queue on server: {inQueueOnServerMessage}");

            Console.WriteLine("LOGS");

            foreach (var logMessage in ConsoleStatusReporterLoggerProvider.messages.OrderBy(m => m.Item1).ToArray())
            {
                Console.WriteLine($"{logMessage.Item1:hh:mm:ss} {logMessage.Item2,-100}");
            }
        }
        private async Task PingStatisticTask(CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromSeconds(20), token);
            while (!token.IsCancellationRequested)
            {
                try
                {
                    this.statistic = await solutionsBase.GetStatistic();
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error while fetch statistic");
                }
            }
        }
    }

    class ConsoleStatusReporterLoggerProvider : ILoggerProvider
    {
        public static (DateTime, string)[] messages = new (DateTime, string)[10];
        private static int currentIndex;

        public ILogger CreateLogger(string categoryName)
        {
            if (categoryName.StartsWith(nameof(Executor)))
            {
                categoryName = categoryName.Substring(categoryName.LastIndexOf(".") + 1);
            }
            return new Logger(categoryName);
        }

        public void Dispose()
        {
        }

        private class Logger : ILogger
        {
            private static Dictionary<LogLevel, string> logNames = new Dictionary<LogLevel, string>
            {
                { LogLevel.None, "NONE" },
                { LogLevel.Trace, "TRC "},
                { LogLevel.Debug, "DEBG"},
                { LogLevel.Information, "INFO" },
                { LogLevel.Warning, "WARN" },
                { LogLevel.Error, "ERR " },
                { LogLevel.Critical, "CRIT" }
            };
            private readonly string category;

            public Logger(string category)
            {
                this.category = category;
            }
            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                messages[Interlocked.Increment(ref currentIndex) % messages.Length] = (DateTime.Now, $"{logNames[logLevel]}| {category}: {formatter(state, exception)}");
            }
        }
    }
}
