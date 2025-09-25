namespace WebDownloader.Application.Errors
{
	public sealed class ValidationException : Exception
	{
		public ValidationException(string message) : base(message) { }
	}
}
