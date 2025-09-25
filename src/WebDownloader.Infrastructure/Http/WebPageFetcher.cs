using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using WebDownloader.Application.Abstractions;
using WebDownloader.Application.Services;

namespace WebDownloader.Infrastructure.Http
{
	public sealed class WebPageFetcher : IWebPageFetcher
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<WebPageFetcher> _logger;
		private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

		public WebPageFetcher(IHttpClientFactory httpClientFactory, ILogger<WebPageFetcher> logger)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;


			_retryPolicy = Policy<HttpResponseMessage>
			.Handle<HttpRequestException>()
			.OrResult(r => (int)r.StatusCode is >= 500 or 408)
			.WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt - 1)),
			onRetry: (outcome, delay, attempt, context) =>
			{
				_logger.LogWarning("Retry {Attempt} after {Delay} for {Url} (Status: {Status})", attempt, delay, context["url"], outcome.Result?.StatusCode);
			});
		}

		public async Task<(HttpStatusCode StatusCode, byte[]? Content, long ElapsedMs)> FetchAsync(Uri uri, CancellationToken ct)
		{
			var client = _httpClientFactory.CreateClient(Counteracts.SERVICE_NAME.ToLower());
			var sw = Stopwatch.StartNew();

			var context = new Context();
			context["url"] = uri.ToString();
			_logger.LogInformation("Fetching {Url}", uri);

			using var response = await _retryPolicy.ExecuteAsync(async (ctx, token) =>
			{
				using var request = new HttpRequestMessage(HttpMethod.Get, uri);
				request.Headers.UserAgent.Add(new ProductInfoHeaderValue("AsyncDownloader", "1.0"));

				return await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
			}, context, ct);

			var code = response.StatusCode;
			byte[]? bytes = null;

			if (response.IsSuccessStatusCode)
			{
				bytes = await response.Content.ReadAsByteArrayAsync(ct);
			}

			sw.Stop();
			_logger.LogInformation("Fetched {Url} with {Status} in {Elapsed}ms, length={Length}", uri, (int)code, sw.ElapsedMilliseconds, bytes?.LongLength);
			return (code, bytes, sw.ElapsedMilliseconds);
		}
	}
}
