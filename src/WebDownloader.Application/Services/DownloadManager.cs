using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using WebDownloader.Application.Abstractions;
using WebDownloader.Application.Errors;
using WebDownloader.Application.Models;
using WebDownloader.Application.Options;

namespace WebDownloader.Application.Services
{
	public sealed class DownloadManager : IDownloadManager
	{
		private readonly IWebPageFetcher _fetcher;
		private readonly IContentStore _store;
		private readonly ILogger<DownloadManager> _logger;
		private readonly DownloaderOptions _options;

		public DownloadManager(IWebPageFetcher fetcher, IContentStore store, ILogger<DownloadManager> logger, DownloaderOptions options)
		{
			_fetcher = fetcher;
			_store = store;
			_logger = logger;
			_options = options;
		}

		public async Task<List<DownloadResult>> DownloadAsync(DownloadRequest request, CancellationToken ct)
		{
			var dop = request.DegreeOfParallelism ?? _options.DefaultDegreeOfParallelism;
			if (dop <= 0)
				throw new ValidationException("DegreeOfParallelism must be greater than 0.");

			var urls = request.Urls.Select(u => new Uri(u)).ToArray();
			_logger.LogInformation("Starting downloads for {Count} URLs with DOP={DOP}", urls.Length, dop);

			var results = new ConcurrentBag<DownloadResult>();
			using var semaphore = new SemaphoreSlim(dop);
			var tasks = urls.Select(async uri =>
			{
				await semaphore.WaitAsync(ct);
				try
				{
					var (statusCode, content, elapsedMs) = await _fetcher.FetchAsync(uri, ct);
					if (content is not null && (int)statusCode >= 200 && (int)statusCode < 300)
					{
						var hash = Convert.ToHexString(SHA256.HashData(content));
						var key = SanitizeKey(uri);
						var path = await _store.SaveAsync(key, content, ct);

						results.Add(new DownloadResult
						{
							Url = uri.ToString(),
							StatusCode = statusCode,
							ContentLength = content.LongLength,
							Sha256 = hash,
							StoredAt = path,
							ElapsedMs = elapsedMs,
							Status = DownloadStatus.Success
						});
					}
					else
					{
						results.Add(new DownloadResult
						{
							Url = uri.ToString(),
							StatusCode = statusCode,
							ContentLength = content?.LongLength,
							ElapsedMs = elapsedMs,
							Status = DownloadStatus.Failed,
							Error = $"HTTP {(int)statusCode}"
						});
					}
				}
				catch (OperationCanceledException)
				{
					_logger.LogWarning("Download cancelled for {Url}", uri);
					results.Add(new DownloadResult
					{
						Url = uri.ToString(),
						StatusCode = 0,
						Status = DownloadStatus.Failed,
						Error = "Cancelled"
					});
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to download {Url}", uri);
					results.Add(new DownloadResult
					{
						Url = uri.ToString(),
						StatusCode = 0,
						Status = DownloadStatus.Failed,
						Error = ex.Message
					});
				}
				finally
				{
					semaphore.Release();
				}
			});


			await Task.WhenAll(tasks);
			_logger.LogInformation("Completed downloads. Success={Success} / Total={Total}", results.Count(r => r.Status == DownloadStatus.Success), urls.Length);
			return results.ToList();
		}

		private static string SanitizeKey(Uri uri)
		{
			// create a stable file-friendly key: host_path_hash
			var raw = uri.ToString();
			var hash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(raw))).Substring(0, 8);
			var host = uri.Host.Replace(':', '_');
			var path = uri.AbsolutePath.Replace('/', '_').Trim('_');
			if (string.IsNullOrWhiteSpace(path)) path = "root";
			return $"{host}_{path}_{hash}";
		}
	}
}
