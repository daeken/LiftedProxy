using System.IO;
using System.Threading.Tasks;

namespace ProxyCore {
	public static class Extensions {
		public static async Task<string> ReadLfLineAsync(this Stream stream) {
			var ret = "";
			var cb = new byte[1];
			while(true) {
				if(await stream.ReadAsync(cb, 0, 1) == 0)
					return ret.Length == 0 ? null : ret;
				if(cb[0] == '\n')
					return ret + "\n";
				ret += (char) cb[0];
			}
		}
	}
}