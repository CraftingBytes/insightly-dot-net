using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace InsightlySDK
{
	public enum HTTPMethod
	{
		GET,
		PUT,
		POST,
		DELETE
	}

	public class InsightlyRequest
	{
		public InsightlyRequest(string api_key, string url_path)
		{
			this.method = HTTPMethod.GET;
			this.api_key = api_key;
			this.url_path = url_path;
			this.query_params = new List<string>();
		}

		public Stream AsInputStream()
		{
			var url = new UriBuilder(BASE_URL);
			url.Path = this.url_path;
			url.Query = this.QueryString;

			var request = WebRequest.Create(url.ToString());
			request.Method = this.method.ToString();

			var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(api_key + ":"));
			request.Headers.Add("Authorization", "Basic " + credentials);

			if (this.body != null)
			{
				request.ContentLength = Encoding.UTF8.GetByteCount(this.body);
				request.ContentType = "application/json";
				var writer = new StreamWriter(request.GetRequestStream());
				writer.Write(this.body);
				writer.Flush();
				writer.Close();
			}

			var response = request.GetResponse();
			return response.GetResponseStream();
		}

		public Object AsJson()
		{
			return this.AsJson<Object>();
		}

		public T AsJson<T>()
		{
			return JsonConvert.DeserializeObject<T>(this.AsString());
		}

		public string AsString()
		{
			return (new StreamReader(this.AsInputStream())).ReadToEnd();
		}

		public InsightlyRequest WithBody(string contents)
		{
			this.body = contents;
			return this;
		}

		public InsightlyRequest WithBody<T>(T body)
		{
			return this.WithBody(JsonConvert.SerializeObject(body));
		}

		public InsightlyRequest WithQueryParam(string name, string value)
		{
			this.query_params.Add(Uri.EscapeDataString(name) + "=" + Uri.EscapeDataString(value));
			return this;
		}

		public InsightlyRequest WithQueryParam(string name, int val)
		{
			this.WithQueryParam(name, val.ToString());
			return this;
		}

		public InsightlyRequest WithQueryParam(string name, List<int> values)
		{
			foreach (var val in values)
			{
				this.WithQueryParam(name, val);
			}
			return this;
		}

		public InsightlyRequest WithMethod(HTTPMethod method)
		{
			this.method = method;
			return this;
		}


		// OData Query building methods

		public InsightlyRequest Top(int n)
		{
			return this.WithQueryParam("top", n.ToString());
		}

		public InsightlyRequest Skip(int n)
		{
			return this.WithQueryParam("skip", n.ToString());
		}

		public InsightlyRequest OrderBy(string order_by)
		{
			return this.WithQueryParam("orderby", order_by);
		}

		public InsightlyRequest Filter(string filter)
		{
			return this.WithQueryParam("filter", filter);
		}

		public InsightlyRequest Filters(ICollection<string> filters)
		{
			foreach (var filter in filters)
			{
				this.Filter(filter);
			}
			return this;
		}

		private string QueryString
		{
			get
			{
				if (query_params.Count > 0)
				{
					return String.Join("&", query_params);
				}
				else
				{
					return "";
				}
			}
		}

		private const string BASE_URL = "https://api.insight.ly";
		private HTTPMethod method;
		private string api_key;
		private string url_path;
		private List<string> query_params;
		private string body;
	}
}