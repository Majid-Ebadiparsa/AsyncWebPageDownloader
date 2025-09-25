using WebDownloader.Api.Configuration;
using WebDownloader.Api.Endpoints;
using WebDownloader.Api.Middleware;
using WebDownloader.Application;
using WebDownloader.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services
	.AddEndpointsApiExplorer()
	.AddCustomSwagger()
	.AddDownloaderApplication(builder.Configuration)
	.AddDownloaderInfrastructure(builder.Configuration);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();



var app = builder.Build();
app
	.UseCustomExceptionHandler()
	.UseHttpsRedirection()
	.UseCustomSwaggerUiExceptionHandler();


app.MapDownloadEndpoints();
app.Run();
