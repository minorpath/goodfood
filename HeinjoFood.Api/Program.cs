using HeinjoFood.Api;
using HeinjoFood.Api.Data;
using HeinjoFood.Api.Models;
using System.Diagnostics;
using System.Reflection;

var startTime = DateTime.UtcNow;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    var basePath = AppContext.BaseDirectory;
    var fileName = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
    var filePath = Path.Combine(basePath, fileName);
    setupAction.IncludeXmlComments(filePath);
});
builder.Services.AddRouting(configureOptions => configureOptions.LowercaseUrls = true);
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));   
builder.Services.AddSingleton<StorageManager>();
builder.Services.AddSingleton<IFileStorage, BlobFileStorage>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", (IWebHostEnvironment env) => {
    var aspNetCoreAssembly = typeof(Microsoft.AspNetCore.Builder.WebApplication).Assembly;
    var fileVersionInfo = FileVersionInfo.GetVersionInfo(aspNetCoreAssembly.Location);
    var baseDirectory = System.AppContext.BaseDirectory;
    return $".NET 6 Minimal API is up and running!" + Environment.NewLine +
            $"AspNetCore Version: {fileVersionInfo.FileVersion} (Location: {aspNetCoreAssembly.Location})" + Environment.NewLine +
            $"Base directory: {baseDirectory}" + Environment.NewLine +
            $"Startup time (UTC): {startTime:o}" + Environment.NewLine +
            $"Environment name: {env.EnvironmentName}" + Environment.NewLine;
}).WithName("RootEndpoint");

app.Run();
