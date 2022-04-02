namespace Executor.Executers.Build
{
    abstract class BuildProperty
    {
        public abstract bool IsCompilationFailed(string logs);
    }
}
