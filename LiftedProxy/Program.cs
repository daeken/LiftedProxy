using System;
using System.Net;
using ProxyCore;

namespace LiftedProxy {
	class Program {
		public static HookProxy Proxy;
		static void Main(string[] args) {
			Proxy = new HookProxy(IPAddress.Any, 12345);
			WebInterface.Setup();
		}
	}
}