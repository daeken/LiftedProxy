using System;
using System.Net;
using ProxyCore;

namespace LiftedProxy {
	class Program {
		static void Main(string[] args) {
			var proxy = new HttpProxy(IPAddress.Any, 12345);
			var webapp = new WebInterface();
		}
	}
}