using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseKinogo
{
	public class Parser
	{
		private HttpClientHandler _handler = new HttpClientHandler();
		private string _pattern;
		private string _uri;

		public Parser(string pattern, string uri, bool useProxy)
		{
			_pattern = pattern;
			_uri = uri;

			if (useProxy)
			{
				EnableProxy();
			}
		}

		private void EnableProxy()
		{
			string address = ProxyServer.List[new Random().Next(1, 395)].ToString();
			WebProxy proxy = new WebProxy(address);
			_handler.Proxy = proxy;
		}

		// best method's name...
		public async Task<IEnumerable<string>> Get()
		{
			try
			{
				HttpResponseMessage response = await new HttpClient(_handler).GetAsync(_uri);
				response.EnsureSuccessStatusCode();

				byte[] bytes = await response.Content.ReadAsByteArrayAsync();
				string responseBodyAsText = Encoding.Default.GetString(bytes);

				Regex reg = new Regex(_pattern);

				return reg.Matches(responseBodyAsText).Cast<Match>().Select(m => m.Value).ToList();
			}
			catch
			{
				return null;
			}
		}
	}
}
