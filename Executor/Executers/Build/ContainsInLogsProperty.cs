namespace Executor.Executers.Build
{
    class ContainsInLogsProperty : DockerBuildImageErrorProperty
    {
        public string BuildFailedCondition { get; set; }

        public override bool IsCompilationFailed(string logs) =>
            base.IsCompilationFailed(logs) ||
            logs?.Contains(BuildFailedCondition) == true;
    }
}
