namespace WebDownloader.Tests
{
	public sealed class TestHttpMessageHandler : HttpMessageHandler
	{
		private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;


		public TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
		{
			_handler = handler;
		}


		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return Task.FromResult(_handler(request));
		}
	}
}
