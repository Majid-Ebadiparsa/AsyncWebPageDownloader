using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using WebDownloader.Application.Abstractions;
using WebDownloader.Application.Models;
using WebDownloader.Application.Options;
using WebDownloader.Application.Services;
using WebDownloader.Infrastructure.Storage;

namespace WebDownloader.Tests
{
	public class DownloadManagerTests
	{
		private sealed class StubFetcher : IWebPageFetcher
		{
			public Task<(HttpStatusCode StatusCode, byte[]? Content, long ElapsedMs)> FetchAsync(Uri uri, CancellationToken ct)
			{
				var content = System.Text.Encoding.UTF8.GetBytes($"content for {uri}");
				return Task.FromResult((HttpStatusCode.OK, content, 5L));
			}
		}


		[Fact]
		public async Task DownloadAsync_ReturnsSuccess_ForValidUrls()
		{
			var fetcher = new StubFetcher();
			var store = new MemoryContentStore();
			var logger = NullLogger<DownloadManager>.Instance;
			var manager = new DownloadManager(
					fetcher,
					store,
					logger,
					new DownloaderOptions { DefaultDegreeOfParallelism = 2 });

			var request = new DownloadRequest { Urls = new() { "https://a.test/1", "https://b.test/2" }, DegreeOfParallelism = 2 };
			var results = await manager.DownloadAsync(request, CancellationToken.None);


			Assert.Equal(2, results.Count);
			Assert.All(results, r => Assert.Equal(DownloadStatus.Success, r.Status));
			Assert.All(results, r => Assert.NotNull(r.Sha256));
		}
	}
}
