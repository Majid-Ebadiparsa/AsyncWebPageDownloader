using WebDownloader.Application.Models;

namespace WebDownloader.Application.Abstractions
{
	public interface IDownloadManager
	{
		Task<List<DownloadResult>> DownloadAsync(DownloadRequest request, CancellationToken ct);
	}
}
