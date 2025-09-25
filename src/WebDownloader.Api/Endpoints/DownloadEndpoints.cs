using WebDownloader.Application.Abstractions;
using WebDownloader.Application.Errors;
using WebDownloader.Application.Models;

namespace WebDownloader.Api.Endpoints
{
	public static class DownloadEndpoints
	{
		public static IEndpointRouteBuilder MapDownloadEndpoints(this IEndpointRouteBuilder endpoints)
		{
			var group = endpoints.MapGroup("/api/downloads");


			group.MapPost("/", async (
			DownloadRequest request,
			IDownloadManager manager,
			ILoggerFactory loggerFactory,
			CancellationToken ct) =>
			{
				Validate(request);
				var logger = loggerFactory.CreateLogger("DownloadEndpoint");
				logger.LogInformation("Received download request for {Count} URLs", request.Urls.Count);


				var results = await manager.DownloadAsync(request, ct);
				return Results.Ok(results);
			})
			.WithName("StartDownload")
			.WithSummary("Starts asynchronous download of multiple URLs and returns aggregated results.")
			.Produces<List<DownloadResult>>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status500InternalServerError);


			return endpoints;
		}


		private static void Validate(DownloadRequest request)
		{
			if (request is null)
				throw new ValidationException("Request body is required.");


			if (request.Urls is null || request.Urls.Count == 0)
				throw new ValidationException("At least one URL is required.");


			foreach (var url in request.Urls)
			{
				if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
				{
					throw new ValidationException($"Invalid URL: {url}");
				}
			}


			if (request.DegreeOfParallelism is <= 0)
				throw new ValidationException("DegreeOfParallelism must be greater than 0.");
		}
	}
}
