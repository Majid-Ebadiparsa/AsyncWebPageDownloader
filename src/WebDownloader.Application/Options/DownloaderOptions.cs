namespace WebDownloader.Application.Options
{
	public sealed record DownloaderOptions
	{
		public int DefaultDegreeOfParallelism { get; init; } = 4;
		public string StorageMode { get; init; } = "Memory";
		public string? StorageDirectory { get; init; }
	}
}
