using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static System.Console;

namespace ProxyCore {
	public class HttpRequest {
		public string EntireRequest { get; set; }
		
		public string Method { get; set; }
		public string URI { get; set; }
		
		public async Task<HttpRequest> Read(Stream stream) {
			var headerLines = new List<string>();
			while(true) {
				var line = await stream.ReadLfLineAsync();
				if(line == null)
					break;
				if(line == "\n" || line == "\r\n")
					break;
				headerLines.Add(line);
			}
			if(headerLines.Count == 0)
				return null;
			WriteLine(headerLines[0]);
			return this;
		}
	}
}