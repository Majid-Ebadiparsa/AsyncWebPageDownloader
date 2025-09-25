using System.Net;

namespace WebDownloader.Application.Abstractions
{
	public interface IWebPageFetcher
	{
		Task<(HttpStatusCode StatusCode, byte[]? Content, long ElapsedMs)> FetchAsync(Uri uri, CancellationToken ct);
	}
}
