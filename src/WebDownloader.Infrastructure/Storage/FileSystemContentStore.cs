using WebDownloader.Application.Abstractions;

namespace WebDownloader.Infrastructure.Storage
{
	/// <summary>
	/// Strategy: stores bytes on disk under configured directory.
	/// </summary>
	public sealed class FileSystemContentStore : IContentStore
	{
		private readonly string _root;


		public FileSystemContentStore(string root)
		{
			_root = root;
			Directory.CreateDirectory(_root);
		}

		public async Task<string> SaveAsync(string key, byte[] content, CancellationToken ct)
		{
			var path = Path.Combine(_root, key + ".bin");
			await File.WriteAllBytesAsync(path, content, ct);
			return path;
		}
	}
}
