using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using WebDownloader.Api.DTOs;

namespace WebDownloader.Api.Configuration
{
	public static class CustomSwaggerExtension
	{
		public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
		{
			// Register the Swagger generator
			services
				.AddEndpointsApiExplorer()
				.AddSwaggerExamplesFromAssemblyOf<LoginRequestExample>()
				.AddSwaggerGen(c =>
				{
					c.SwaggerDoc("v1", new OpenApiInfo
					{
						Title = "WebDownloader.API",
						Version = "V1",
						Description = "Web Downloader Demo (WDD)",
					});

					c.ExampleFilters();
				});

			return services;
		}

	}
}
