using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using WebApp;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", true);
builder.Configuration.AddJsonFile("appsettings.Build.json", true);

builder.WebHost.UseSentry();

Startup.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

Startup.ConfigurePipeline(app, builder.Environment, builder.Configuration);

app.Run();

