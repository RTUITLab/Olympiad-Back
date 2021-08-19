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

var resultsViewerPublishDir = "deploy/results-viewer/results-viewer-build";
var resultsViewerProject = "ResultsViewer/ResultsViewer.csproj";

var controlPanelPublishDir = "deploy/control-panel/control-panel-build";
var controlPanelProject = "ControlPanel/ControlPanel.csproj";

Setup(ctx =>
{
   CleanDirectory(apiPublishDir);
   CleanDirectory(executorPublishDir);
   CleanDirectory(adminPublishDir);
   CleanDirectory(resultsViewerPublishDir);
   CleanDirectory(controlPanelPublishDir);
});


Task("RestoreSolution")
   .Does(() =>
{
   DotNetCoreRestore(apiProject);
   DotNetCoreRestore(executorProject);
   DotNetCoreRestore(adminProject);
   DotNetCoreRestore(resultsViewerProject);
   DotNetCoreRestore(controlPanelProject);
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

Task("BuildResultsViewer")
   .IsDependentOn("RestoreSolution")
   .Does(() =>
{
   var settings = new DotNetCoreBuildSettings {
      Configuration = configuration
   };
   DotNetCoreBuild(resultsViewerProject, settings);
});

Task("PublishResultsViewer")
   .IsDependentOn("BuildResultsViewer")
   .Does(() =>
{
   var settings = new DotNetCorePublishSettings
   {
      Configuration = configuration,
      OutputDirectory = resultsViewerPublishDir
   };

   DotNetCorePublish(resultsViewerProject, settings);
});

Task("BuildControlPanel")
   .IsDependentOn("RestoreSolution")
   .Does(() =>
{
   var settings = new DotNetCoreBuildSettings {
      Configuration = configuration
   };
   DotNetCoreBuild(controlPanelProject, settings);
});

Task("PublishControlPanel")
   .IsDependentOn("BuildControlPanel")
   .Does(() =>
{
   var settings = new DotNetCorePublishSettings
   {
      Configuration = configuration,
      OutputDirectory = controlPanelPublishDir
   };

   DotNetCorePublish(controlPanelProject, settings);
});

Task("PublishAll")
   .IsDependentOn("PublishApi")
   .IsDependentOn("PublishExecutor")
   .IsDependentOn("PublishAdmin")
   .IsDependentOn("PublishResultsViewer")
   .IsDependentOn("PublishControlPanel")
   .Does(() =>
{
   
});

RunTarget(target);