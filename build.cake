var target = Argument("target", "PublishAll");
var configuration = Argument("configuration", "Release");

var apiDeployDir = "deploy/api";
var apiPublishDir = apiDeployDir + "/api-build";
var apiProject = "WebApp/WebApp.csproj";

var apiTestsDir = "tests/e2e/api";

var executorPublishDir = "deploy/executor/executor-build";
var executorProject = "Executor/Executor.csproj";

var controlPanelPublishDir = "deploy/control-panel/control-panel-build";
var controlPanelProjectDir = "ControlPanel";
var controlPanelProject = controlPanelProjectDir + "/ControlPanel.csproj";

Setup(ctx =>
{
   CleanDirectory(apiPublishDir);
   CleanDirectory(executorPublishDir);
   CleanDirectory(controlPanelPublishDir);
});


Task("RestoreSolution")
   .Does(() =>
{
   DotNetTool("tool restore");

   DotNetRestore(apiProject);
   DotNetRestore(executorProject);

   DotNetRestore(controlPanelProject);
});

Task("BuildApi")
   .IsDependentOn("RestoreSolution")
   .Does(() =>
{
   var settings = new DotNetBuildSettings {
      Configuration = configuration
   };
   DotNetBuild(apiProject, settings);
});

Task("PublishApi")
   .IsDependentOn("BuildApi")
   .Does(() =>
{
   var settings = new DotNetPublishSettings
   {
      Configuration = configuration,
      OutputDirectory = apiPublishDir
   };

   DotNetPublish(apiProject, settings);

   System.IO.File.WriteAllText(apiPublishDir + "/appsettings.Build.json",
      System.Text.Json.JsonSerializer.Serialize(new { 
         About = new {
            BuildNumber = EnvironmentVariable<string>("BUILD_NUMBER", "no-build-id")
         } 
      })
      );

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
   var settings = new DotNetBuildSettings {
      Configuration = configuration
   };
   DotNetBuild(executorProject, settings);
});

Task("PublishExecutor")
   .IsDependentOn("BuildExecutor")
   .Does(() =>
{
   var settings = new DotNetPublishSettings
   {
      Configuration = configuration,
      OutputDirectory = executorPublishDir
   };

   DotNetPublish(executorProject, settings);
});

Task("BuildControlPanel")
   .IsDependentOn("RestoreSolution")
   .Does(() =>
{
   var settings = new DotNetBuildSettings {
      Configuration = configuration
   };
   DotNetBuild(controlPanelProject, settings);
});

Task("PublishControlPanel")
   .IsDependentOn("BuildControlPanel")
   .Does(() =>
{
   var settings = new DotNetPublishSettings
   {
      Configuration = configuration,
      OutputDirectory = controlPanelPublishDir
   };

   DotNetPublish(controlPanelProject, settings);
});

Task("PublishAll")
   .IsDependentOn("PublishApi")
   .IsDependentOn("PublishExecutor")
   .IsDependentOn("PublishControlPanel")
   .Does(() =>
{
   
});

RunTarget(target);