using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bridge.Html5;
using Bridge.jQuery2;
using Newtonsoft.Json;
using static System.Console;

namespace Client {
	public static class Program {
		public static string EncodeUriComponents(Dictionary<string, string> comp) {
			string ret = null;
			foreach(var elem in comp)
				ret = (ret ?? "") + Window.EncodeURIComponent(elem.Key) + "=" + Window.EncodeURIComponent(elem.Value) + "&";
			return ret?.Substring(0, ret.Length - 1) ?? "";
		}
		
		public static Task<T> FetchJson<T>(string path, Dictionary<string, string> args = null) where T : class {
			var tcs = new TaskCompletionSource<T>();
			var req = new XMLHttpRequest();
			req.Open("GET", path + (args != null ? "?" + EncodeUriComponents(args) : ""));
			req.OnReadyStateChange = () => {
				if(req.ReadyState != AjaxReadyState.Done)
					return;
				try {
					if(req.Status == 200)
						tcs.SetResult(JsonConvert.DeserializeObject<T>(req.ResponseText));
					else
						tcs.SetException(new Exception("Response code does not indicate success: " + req.StatusText));
				} catch(Exception e) {
					tcs.SetException(e);
				}
			};
			req.Send();
			return tcs.Task;
		}

		public class ProxyHistory {
			public class HttpRequest {
				public string method, uri, headerText, dateTime;
				public byte[] body;
			}

			public class HttpResponse {
				public string headerText, dateTime;
				public byte[] body;
			}

			public HttpRequest httpRequest;
			public HttpResponse httpResponse;
			
			public static async Task<ProxyHistory[]> Fetch() {
				return await FetchJson<ProxyHistory[]>("/api/proxyHistory");
			}
		}
		
		[Ready]
		public static async void Main() {
			var table = new TableComponent(new [] {"URI", "Time"});
			new jQuery(Document.Body).Append(new PaneComponent("History") { Content = table});
			foreach(var elem in await ProxyHistory.Fetch())
				table.AddRow(elem.httpRequest.uri, elem.httpRequest.dateTime);
		}
	}
}