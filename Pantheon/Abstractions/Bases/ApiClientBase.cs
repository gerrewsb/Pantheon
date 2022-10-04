using Polly;
using Serilog;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Pantheon.Abstractions.Bases
{
	/// <summary>
	/// Base for all ApiClients used in .NET frontends
	/// </summary>
	public abstract class ApiClientBase
	{
		private readonly HttpClient _httpClient;
		private readonly Func<Task<string>> _getToken;
		private readonly ILogger _logger;
		private string? _token;
		private readonly JsonSerializerOptions? _jsonSerializerOptions;

		/// <summary>
		/// Base constructor with default 1 minute timeout
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="tokenDelegate">A function to retrieve or refresh a token to authenticate the endpoint</param>
		/// <param name="logger">A Serilog logger</param>
		/// <param name="jsonSerializerOptions">Optional JsonSerializerOptions to serialize and deserialize requests and responses</param>
		public ApiClientBase(HttpClient client, Func<Task<string>> tokenDelegate, ILogger logger, JsonSerializerOptions? jsonSerializerOptions = null)
			: this(client, tokenDelegate, logger, new TimeSpan(0, 1, 0), jsonSerializerOptions)
		{ }

		/// <summary>
		/// Base constructor with variable timeout
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="tokenDelegate">A function to retrieve or refresh a token to authenticate the endpoint</param>
		/// <param name="logger">A Serilog logger</param>
		/// <param name="timeout">timeout for the HttpClient</param>
		/// <param name="jsonSerializerOptions"></param>
		public ApiClientBase(HttpClient client, Func<Task<string>> tokenDelegate, ILogger logger, TimeSpan timeout, JsonSerializerOptions? jsonSerializerOptions = null)
		{
			_httpClient = client;
			_getToken = tokenDelegate;
			_logger = logger;
			_jsonSerializerOptions = jsonSerializerOptions;
			_httpClient.Timeout = timeout;
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
		}

		/// <summary>
		/// Creates a HttpRequestMessage to be used by the HttpClient
		/// </summary>
		/// <param name="method">HttpMethod</param>
		/// <param name="url">The url of the API endpoint</param>
		/// <param name="body">Optional body for POST/PUT requests</param>
		/// <returns>
		/// This method returns an object of type <see cref="HttpRequestMessage"/>
		/// based on the <see cref="HttpMethod"/>, url and optional body
		/// </returns>
		public HttpRequestMessage CreateRequest(HttpMethod method, string url, object? body = null)
		{
			HttpRequestMessage request = new(method, url);

			if (method == HttpMethod.Post || method == HttpMethod.Put)
			{
				request.Content = CreateHttpRequestContent(body);
			}

			return request;
		}

		/// <summary>
		/// Used to send a HttpRequestMessage to the API.
		/// This method implements a retrypolicy of 3 tries with 3 seconds between tries.
		/// </summary>
		/// <param name="request"><see cref="HttpRequestMessage"/></param>
		/// <param name="completionOption"><see cref="HttpCompletionOption"/> defaults to <see cref="HttpCompletionOption.ResponseHeadersRead"/></param>
		/// <param name="allowAnonymous">If an endpoint doesn't require an authenticated token, this parameter can be set to <see cref="true"/> to bypass the TokenDelegate</param>
		/// <param name="cancellationToken"><see cref="CancellationToken"/> to cancel the request if needed</param>
		/// <exception cref="HttpRequestException">This method throws an <see cref="HttpRequestException"/> if the response doens't have a success statuscode.</exception>
		public async Task SendToApi(HttpRequestMessage request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead, bool allowAnonymous = false, CancellationToken cancellationToken = default)
		{
			if (!allowAnonymous)
			{
				_token = await _getToken();
			}

			if (!string.IsNullOrWhiteSpace(_token) && !allowAnonymous)
			{
				request.Headers.Authorization = new("Bearer", _token);
			}

			using (HttpResponseMessage response = await Policy
					.HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode && message.StatusCode != HttpStatusCode.BadRequest && message.StatusCode != HttpStatusCode.NotFound)
					.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(3), (response, timespan, retryCount, _) =>
						_logger.Error("Request failed with statuscode: {0}. Waiting {1} seconds before next retry. Retry attempt: {2}", response.Result.StatusCode, timespan.Seconds, retryCount))
					.ExecuteAsync(() => _httpClient.SendAsync(CloneRequest(request), completionOption, cancellationToken)))
			{
				string responseMessage = await response.Content.ReadAsStringAsync(cancellationToken);

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpRequestException(responseMessage, null, response.StatusCode);
				}
			}
		}

		/// <summary>
		/// Used to send a HttpRequestMessage to the API and deserializes the response to the requested type.
		/// This method implements a retrypolicy of 3 tries with 3 seconds between tries.
		/// </summary>
		/// <param name="request"><see cref="HttpRequestMessage"/></param>
		/// <param name="completionOption"><see cref="HttpCompletionOption"/> defaults to <see cref="HttpCompletionOption.ResponseHeadersRead"/></param>
		/// <param name="allowAnonymous">If an endpoint doesn't require an authenticated token, this parameter can be set to <see cref="true"/> to bypass the TokenDelegate</param>
		/// <param name="cancellationToken"><see cref="CancellationToken"/> to cancel the request if needed</param>
		/// <returns>This method return a deserialized version of <see cref="T"/> if the response had a success statuscode.</returns>
		/// <exception cref="HttpRequestException">This method throws an <see cref="HttpRequestException"/> if the response doens't have a success statuscode.</exception>
		public async Task<T> SendToApi<T>(HttpRequestMessage request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead, bool allowAnonymous = false, CancellationToken cancellationToken = default)
		{
			if (!allowAnonymous)
			{
				_token = await _getToken();
			}

			if (!string.IsNullOrWhiteSpace(_token) && !allowAnonymous)
			{
				request.Headers.Authorization = new("Bearer", _token);
			}

			using (HttpResponseMessage response = await Policy
					.HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode && message.StatusCode != HttpStatusCode.BadRequest && message.StatusCode != HttpStatusCode.NotFound)
					.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(3), (response, timespan, retryCount, _) =>
						_logger.Error("Request failed with statuscode: {0}. Waiting {1} seconds before next retry. Retry attempt: {2}", response.Result.StatusCode, timespan.Seconds, retryCount))
					.ExecuteAsync(() => _httpClient.SendAsync(CloneRequest(request), completionOption, cancellationToken)))
			{
				string responseMessage = await response.Content.ReadAsStringAsync(cancellationToken);

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpRequestException(responseMessage, null, response.StatusCode);
				}

				return JsonSerializer.Deserialize<T>(responseMessage, _jsonSerializerOptions)!;
			}
		}

		private StringContent CreateHttpRequestContent(object? body) => new(JsonSerializer.Serialize(body, _jsonSerializerOptions), Encoding.UTF8, "application/json");

		private HttpRequestMessage CloneRequest(HttpRequestMessage request)
		{
			HttpRequestMessage clonedRequest = new(request.Method, request.RequestUri);

			if (request.Content != null)
			{
				clonedRequest.Content = request.Content;
			}

			clonedRequest.Version = request.Version;

			foreach (KeyValuePair<string, object?> option in request.Options)
			{
				clonedRequest.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);
			}

			foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
			{
				clonedRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
			}

			return clonedRequest;
		}
	}
}
