
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProxyCore {
	public class HeaderDictionary {
		public List<Tuple<string, string>> Headers = new List<Tuple<string, string>>();

		public List<string> this[string key] {
			get => Headers.Where(x => x.Item1.ToLower() == key.ToLower()).Select(x => x.Item2).ToList();
			set => Headers = Headers.Where(x => x.Item1.ToLower() != key.ToLower()).Concat(value.Select(x => Tuple.Create(key, x))).ToList();
		}

		public HeaderDictionary() {
		}

		public HeaderDictionary(List<Tuple<string, string>> headers) {
			Headers = headers;
		}

		public string Get(string key, string @default=null) {
			foreach(var elem in Headers)
				if(elem.Item1.ToLower() == key.ToLower())
					return elem.Item2;
			return @default;
		}

		public void Add(string key, string value) {
			Headers.Add(Tuple.Create(key, value));
		}

		public override string ToString() {
			var ret = "";
			foreach(var elem in Headers)
				ret += $"- '{elem.Item1}' : '{elem.Item2}'\n";
			return ret.Substring(0, ret.Length > 0 ? ret.Length - 1 : 0);
		}
	}
}