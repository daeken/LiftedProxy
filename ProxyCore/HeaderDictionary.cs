
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProxyCore {
	public class HeaderDictionary {
		List<Tuple<string, string>> _headers = new List<Tuple<string, string>>();

		public List<string> this[string key] {
			get => _headers.Where(x => x.Item1.ToLower() == key.ToLower()).Select(x => x.Item2).ToList();
			set => _headers = _headers.Where(x => x.Item1.ToLower() != key.ToLower()).Concat(value.Select(x => Tuple.Create(key, x))).ToList();
		}

		public string Get(string key, string @default=null) {
			foreach(var elem in _headers)
				if(elem.Item1.ToLower() == key.ToLower())
					return elem.Item2;
			return @default;
		}

		public void Add(string key, string value) {
			_headers.Add(Tuple.Create(key, value));
		}

		public override string ToString() {
			var ret = "";
			foreach(var elem in _headers)
				ret += $"- '{elem.Item1}' : '{elem.Item2}'\n";
			return ret.Substring(0, ret.Length > 0 ? ret.Length - 1 : 0);
		}
	}
}