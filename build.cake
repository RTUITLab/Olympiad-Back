var target = Argument("target", "PublishApi");
var configuration = Argument("configuration", "Release");

var apiPublishDir = "deploy/api/api-build";
var apiProject = "WebApp/WebApp.csproj";

Setup(ctx =>
{
   CleanDirectory(apiPublishDir);
});


Task("RestoreSolurion")
   .Does(() =>
{
   DotNetCoreRestore();
});

Task("BuildApi")
   .Does(() =>
{
   var settings = new DotNetCoreBuildSettings {
      Configuration = configuration
   };
   DotNetCoreBuild(apiProject, settings);
});

Task("PublishApi")
   .Does(() =>
{
   var settings = new DotNetCorePublishSettings
   {
      Configuration = configuration,
      OutputDirectory = apiPublishDir
   };

   DotNetCorePublish(apiProject, settings);
});

RunTarget(target);