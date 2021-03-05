var target = Argument("target", "PublishAll");
var configuration = Argument("configuration", "Release");

var apiDeployDir = "deploy/api";
var apiPublishDir = apiDeployDir + "/api-build";
var apiProject = "WebApp/WebApp.csproj";

var apiTestsDir = "tests/e2e/api";

var executorPublishDir = "deploy/executor/executor-build";
var executorProject = "Executor/Executor.csproj";

var adminPublishDir = "deploy/admin/admin-build";
var adminProject = "Admin/Admin.csproj";


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

Task("PublishApiToTests")
   .IsDependentOn("PublishApi")
   .Does(() =>
{
   CopyDirectory(apiDeployDir, apiTestsDir);
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

Task("BuildAdmin")
   .IsDependentOn("RestoreSolution")
   .Does(() =>
{
   var settings = new DotNetCoreBuildSettings {
      Configuration = configuration
   };
   DotNetCoreBuild(adminProject, settings);
});

Task("PublishAdmin")
   .IsDependentOn("BuildAdmin")
   .Does(() =>
{
   var settings = new DotNetCorePublishSettings
   {
      Configuration = configuration,
      OutputDirectory = adminPublishDir
   };

   DotNetCorePublish(adminProject, settings);
});

Task("PublishAll")
   .IsDependentOn("PublishApi")
   .IsDependentOn("PublishExecutor")
   .IsDependentOn("PublishAdmin")
   .Does(() =>
{
   
});

RunTarget(target);