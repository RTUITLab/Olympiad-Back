namespace Executor.Executers.Build
{
    abstract class BuildProperty
    {
        public string ProgramFileName { get; set; }
        public abstract bool IsCompilationFailed(string logs);
    }
}
