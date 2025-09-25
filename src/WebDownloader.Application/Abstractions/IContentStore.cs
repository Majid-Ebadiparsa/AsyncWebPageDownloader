namespace WebDownloader.Application.Abstractions
{
	public interface IContentStore
	{
		Task<string> SaveAsync(string key, byte[] content, CancellationToken ct);
	}
}
