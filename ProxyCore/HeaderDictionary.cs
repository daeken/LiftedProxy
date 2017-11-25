
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProxyCore {
	public class HeaderDictionary {
		public List<(string Key, string Value)> Headers = new List<(string, string)>();

		public List<string> this[string key] {
			get => Headers.Where(x => x.Key.ToLower() == key.ToLower()).Select(x => x.Value).ToList();
			set => Headers = Headers.Where(x => x.Key.ToLower() != key.ToLower()).Concat(value.Select(x => (key, x))).ToList();
		}

		public HeaderDictionary() {
		}

		public HeaderDictionary(List<(string Key, string Value)> headers) {
			Headers = headers;
		}

		public string Get(string key, string @default=null) {
			foreach(var elem in Headers)
				if(elem.Key.ToLower() == key.ToLower())
					return elem.Value;
			return @default;
		}

		public void Add(string key, string value) {
			Headers.Add((key, value));
		}

		public override string ToString() {
			var ret = "";
			foreach(var elem in Headers)
				ret += $"- '{elem.Key}' : '{elem.Value}'\n";
			return ret.Substring(0, ret.Length > 0 ? ret.Length - 1 : 0);
		}
	}
}