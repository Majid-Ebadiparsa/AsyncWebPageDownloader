using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebDownloader.Application.Options;
using WebDownloader.Application.Services;

namespace WebDownloader.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddDownloaderApplication(this IServiceCollection services, IConfiguration cfg)
		{
			// Options
			var opts = new DownloaderOptions();
			cfg.GetSection(Counteracts.SERVICE_NAME).Bind(opts);
			services.AddSingleton(opts);

			return services;
		}
	}
}
