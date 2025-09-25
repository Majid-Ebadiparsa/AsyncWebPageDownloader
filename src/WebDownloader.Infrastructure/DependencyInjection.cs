using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebDownloader.Application.Abstractions;
using WebDownloader.Application.Options;
using WebDownloader.Application.Services;
using WebDownloader.Infrastructure.Http;
using WebDownloader.Infrastructure.Storage;

namespace WebDownloader.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddDownloaderInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddHttpClient(Counteracts.SERVICE_NAME.ToLower(), client =>
			{
				client.Timeout = TimeSpan.FromSeconds(20);
				client.DefaultRequestHeaders.ConnectionClose = false;
			});

			services.AddScoped<IWebPageFetcher, WebPageFetcher>();
			services.AddScoped<IDownloadManager, DownloadManager>();


			// Strategy selection via options
			services.AddSingleton<IContentStore>(sp =>
			{
				var opts = sp.GetRequiredService<DownloaderOptions>();
				if (string.Equals(opts.StorageMode, "File", StringComparison.OrdinalIgnoreCase))
				{
					var dir = string.IsNullOrWhiteSpace(opts.StorageDirectory)
							? Path.Combine(AppContext.BaseDirectory, "downloads")
							: opts.StorageDirectory!;
					Directory.CreateDirectory(dir);
					return new FileSystemContentStore(dir);
				}
				return new MemoryContentStore();
			});


			return services;
		}
	}
}
