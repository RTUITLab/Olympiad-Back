namespace Executor.Executers.Build
{
    class ContainsInLogsProperty : BuildProperty
    {
        public string BuildFailedCondition { get; set; }

        public override bool IsCompilationFailed(string logs) => logs?.Contains(BuildFailedCondition) == true;
    }
}
