using System;
namespace Executor.Logging
{
    internal class Logger<T>
    {
        private static Loglevel loglevel = Loglevel.Information; 
        public static void SetLogLevel(Loglevel level) 
            => loglevel = level;
        public static Logger<T> CreateLogger()
        {
            return new Logger<T>();
        }
        Type targetType;
        private Logger()
        {
            targetType = typeof(T);
        }
        public void LogDebug(string message)
            => Log(Loglevel.Debug, message);
        public void LogInformation(string message)
            => Log(Loglevel.Information, message);

        public void Log(Loglevel level, string message)
        {
            if (level < loglevel)
                return;
            System.Console.WriteLine($"{level} {targetType.FullName}: {message}");
        }
        
    }

}