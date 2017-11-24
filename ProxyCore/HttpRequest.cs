using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static System.Console;

namespace ProxyCore {
	public class HttpChunk {
		public string HeaderText { get; set; }
		public HeaderDictionary Headers { get; private set; }
		public byte[] Body { get; set; }
		
		protected async Task ReadFrom(Stream stream) {
			HeaderText = ""; 
			Headers = new HeaderDictionary();
			var first = true;
			while(true) {
				var line = await stream.ReadLfLineAsync();
				if(line == null)
					break;
				if(line == "\n" || line == "\r\n") {
					HeaderText += line;
					break;
				}

				if(first) {
					line = ParseFirst(line);
					first = false;
				} else if(line.Contains(":")) {
					var key = line.Substring(0, line.IndexOf(':'));
					var voff = key.Length + 1 + (line[key.Length + 1] == ' ' ? 1 : 0);
					var value = line.Substring(voff, line.Length - voff - 2);
					Headers.Add(key, value);
				}
				HeaderText += line;
			}
			var clen = Headers.Get("Content-Length");
			if(clen != null && int.TryParse(clen, out var plen)) {
				Body = new byte[plen];
				await stream.ReadAsync(Body, 0, plen);
			}
		}

		protected virtual string ParseFirst(string line) {
			return line;
		}
	}
	
	public class HttpRequest : HttpChunk {
		public string Method { get; set; }
		public string Uri { get; set; }

		public Tuple<string, ushort> TargetHost { get; private set; }

		public static async Task<HttpRequest> Read(Stream stream) {
			var req = new HttpRequest();
			await req.ReadFrom(stream);
			return req;
		}

		protected override string ParseFirst(string line) {
			var sub = line.Split(' ');
			Method = sub[0];
			Uri = sub[1];

			var suri = Uri.Split('/');
			var host = suri[2];
			var port = (ushort) (Uri[4] == 's' || Uri[4] == 'S' ? 443 : 80);
			if(host.Contains(":")) {
				var temp = host.Split(':');
				host = temp[0];
				port = ushort.Parse(temp[1]);
			}
			TargetHost = Tuple.Create(host, port);

			return Method + " " + line.Substring(Method.Length + 1 + suri[0].Length + suri[1].Length + suri[2].Length + 2);
		}

		public override string ToString() {
			return $"HttpRequest Method: '{Method}' Uri: '{Uri}' Headers: '{Headers}'";
		}
	}

	public class HttpResponse : HttpChunk {
		public new static async Task<HttpResponse> Read(Stream stream) {
			var resp = new HttpResponse();
			await resp.ReadFrom(stream);
			return resp;
		}

		public override string ToString() {
			return $"HttpResponse Headers: '{Headers}'";
		}
	}
}