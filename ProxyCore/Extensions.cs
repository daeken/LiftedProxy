using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

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

		public static Task StartNewSafe(this TaskFactory factory, Action cb, TaskCreationOptions opt=TaskCreationOptions.None) {
			var tb = Environment.StackTrace;
			return factory.StartNew(() => {
				try {
					cb();
				} catch(Exception e) {
					WriteLine("Exception in task started from:");
					WriteLine(tb);
					WriteLine($"Inner exception: {e}");
					WriteLine(e.StackTrace);
				}
			}, opt);
		}

		public static Task WriteAsync(this Stream stream, byte[] data) {
			return stream.WriteAsync(data, 0, data.Length);
		}

		public static Task WriteAsync(this Stream stream, string data, Encoding encoding=null) {
			encoding = encoding ?? Encoding.ASCII;
			return stream.WriteAsync(encoding.GetBytes(data));
		}
	}
}