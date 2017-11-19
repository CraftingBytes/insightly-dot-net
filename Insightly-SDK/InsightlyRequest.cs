using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InsightlySDK
{
	public enum HttpMethod
	{
		Get,
		Put,
		Post,
		Delete
	}

	public class InsightlyRequest
	{
		private const string BASE_URL = "https://api.insight.ly";
		private HttpMethod _method;
		private readonly string _apiKey;
		private readonly string _urlPath;
		private readonly List<string> _queryParams;
		private string _body;

		public InsightlyRequest(string apiKey, string urlPath)
		{
			_method = HttpMethod.Get;
			_apiKey = apiKey;
			_urlPath = urlPath;
			_queryParams = new List<string>();
		}

		public async Task<Stream> AsInputStreamAsync()
		{
			var url = new UriBuilder(BASE_URL)
			{
				Path = _urlPath,
				Query = QueryString
			};

			var request = WebRequest.Create(url.ToString());
			request.Method = _method.ToString();

			var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(_apiKey + ":"));
			request.Headers.Add("Authorization", "Basic " + credentials);

			if (_body != null)
			{
				request.ContentLength = Encoding.UTF8.GetByteCount(_body);
				request.ContentType = "application/json";
				var writer = new StreamWriter(request.GetRequestStream());
				writer.Write(_body);
				writer.Flush();
				writer.Close();
			}

			var response = await GetResponseAsync(request);
			return response.GetResponseStream();
		}

		public static Task<Stream> GetRequestStreamAsync(WebRequest request)
		{
			return Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null);
		}
		public static Task<WebResponse> GetResponseAsync(WebRequest request)
		{
			return Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);
		}

		public async Task<object> AsJsonAsync()
		{
			return await AsJsonAsync<Object>();
		}

		public async Task<T> AsJsonAsync<T>()
		{
			var asString = await AsStringAsync();
			return JsonConvert.DeserializeObject<T>(asString);
		}

		public async Task<string> AsStringAsync()
		{
			var stream = await AsInputStreamAsync();
			return await new StreamReader(stream).ReadToEndAsync();
		}

		public InsightlyRequest WithBody(string contents)
		{
			_body = contents;
			return this;
		}

		public InsightlyRequest WithBody<T>(T body)
		{
			return WithBody(JsonConvert.SerializeObject(body));
		}

		public InsightlyRequest WithQueryParam(string name, string value)
		{
			_queryParams.Add(Uri.EscapeDataString(name) + "=" + Uri.EscapeDataString(value));
			return this;
		}

		public InsightlyRequest WithQueryParam(string name, int val)
		{
			WithQueryParam(name, val.ToString());
			return this;
		}

		public InsightlyRequest WithQueryParam(string name, List<int> values)
		{
			foreach (var val in values)
			{
				WithQueryParam(name, val);
			}
			return this;
		}

		public InsightlyRequest WithMethod(HttpMethod method)
		{
			_method = method;
			return this;
		}


		// OData Query building methods

		public InsightlyRequest Top(int n)
		{
			return WithQueryParam("top", n.ToString());
		}

		public InsightlyRequest Skip(int n)
		{
			return WithQueryParam("skip", n.ToString());
		}

		public InsightlyRequest OrderBy(string orderBy)
		{
			return WithQueryParam("orderby", orderBy);
		}

		public InsightlyRequest Filter(string filter)
		{
			return WithQueryParam("filter", filter);
		}

		public InsightlyRequest Filters(ICollection<string> filters)
		{
			foreach (var filter in filters)
			{
				Filter(filter);
			}
			return this;
		}

		private string QueryString
		{
			get
			{
				if (_queryParams.Count > 0)
				{
					return String.Join("&", _queryParams);
				}
				return "";
			}
		}
	}
}