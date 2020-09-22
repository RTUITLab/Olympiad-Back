var target = Argument("target", "PublishAll");
var configuration = Argument("configuration", "Release");

var apiPublishDir = "deploy/api/api-build";
var apiProject = "WebApp/WebApp.csproj";

var executorPublishDir = "deploy/executor/executor-build";
var executorProject = "Executor/Executor.csproj";

Setup(ctx =>
{
   CleanDirectory(apiPublishDir);
   CleanDirectory(executorPublishDir);
});


Task("RestoreSolution")
   .Does(() =>
{
   DotNetCoreRestore();
});

Task("BuildApi")
   .IsDependentOn("RestoreSolution")
   .Does(() =>
{
   var settings = new DotNetCoreBuildSettings {
      Configuration = configuration
   };
   DotNetCoreBuild(apiProject, settings);
});

Task("PublishApi")
   .IsDependentOn("BuildApi")
   .Does(() =>
{
   var settings = new DotNetCorePublishSettings
   {
      Configuration = configuration,
      OutputDirectory = apiPublishDir
   };

   DotNetCorePublish(apiProject, settings);
});

Task("BuildExecutor")
   .IsDependentOn("RestoreSolution")
   .Does(() =>
{
   var settings = new DotNetCoreBuildSettings {
      Configuration = configuration
   };
   DotNetCoreBuild(executorProject, settings);
});

Task("PublishExecutor")
   .IsDependentOn("BuildExecutor")
   .Does(() =>
{
   var settings = new DotNetCorePublishSettings
   {
      Configuration = configuration,
      OutputDirectory = executorPublishDir
   };

   DotNetCorePublish(executorProject, settings);
});

Task("PublishAll")
   .IsDependentOn("PublishApi")
   .IsDependentOn("PublishExecutor")
   .Does(() =>
{
   
});

RunTarget(target);