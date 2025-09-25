using System.Text.Json.Serialization;

namespace WebDownloader.Application.Models
{
	public sealed class DownloadRequest
	{
		public List<string> Urls { get; init; } = new();

		/// <summary>
		/// Max concurrent downloads. If null, uses configured default.
		/// </summary>
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? DegreeOfParallelism { get; init; }
	}
}
