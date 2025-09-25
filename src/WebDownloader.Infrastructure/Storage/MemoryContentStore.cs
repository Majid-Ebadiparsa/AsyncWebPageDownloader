using System.Collections.Concurrent;
using WebDownloader.Application.Abstractions;

namespace WebDownloader.Infrastructure.Storage
{
	/// <summary>
	/// Strategy: stores content in-memory (for demos/tests). Returns a pseudo path key.
	/// </summary>
	public sealed class MemoryContentStore : IContentStore
	{
		private static readonly ConcurrentDictionary<string, byte[]> _store = new();


		public Task<string> SaveAsync(string key, byte[] content, CancellationToken ct)
		{
			_store[key] = content;
			return Task.FromResult($"memory://{key}");
		}
	}
}
