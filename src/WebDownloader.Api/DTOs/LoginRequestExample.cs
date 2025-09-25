using Swashbuckle.AspNetCore.Filters;
using WebDownloader.Application.Models;

namespace WebDownloader.Api.DTOs
{
	public class LoginRequestExample : IExamplesProvider<DownloadRequest>
	{
		public DownloadRequest GetExamples()
		{
			return new DownloadRequest()
			{
				Urls = ["https://www.google.com",
								"https://www.wikipedia.org"],
				DegreeOfParallelism = 4
			};
		}
	}
}
