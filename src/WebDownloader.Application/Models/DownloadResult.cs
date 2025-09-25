using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebDownloader.Application.Models
{
	public sealed class DownloadResult
	{
		public required string Url { get; init; }
		public HttpStatusCode StatusCode { get; init; }
		public long? ContentLength { get; init; }
		public string? Sha256 { get; init; }
		public string? StoredAt { get; init; }
		public long ElapsedMs { get; init; }
		public DownloadStatus Status { get; init; }
		public string? Error { get; init; }
	}
}
