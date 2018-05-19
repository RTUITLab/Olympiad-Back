using System;
namespace Executor.Logging
{
    internal class Logger<T>
    {
        private static Loglevel loglevel = Loglevel.Trace; 
        public static void SetLogLevel(Loglevel level) 
            => loglevel = level;
        public static Logger<T> CreateLogger()
            => CreateLogger("");
        public static Logger<T> CreateLogger(string startMessage)
        {
            return new Logger<T>(startMessage);
        }
        Type targetType;
        private readonly string startMessage;

        private Logger(string startMessage)
        {
            targetType = typeof(T);
            this.startMessage = startMessage;
        }
        public void LogTrace(string message)
            => Log(Loglevel.Trace, message);
        public void LogDebug(string message)
            => Log(Loglevel.Debug, message);
        public void LogInformation(string message)
            => Log(Loglevel.Information, message);

        public void Log(Loglevel level, string message)
        {
            if (level < loglevel)
                return;
            System.Console.WriteLine($"{level} {startMessage} {targetType.FullName}: {message}");
        }
        
    }

}