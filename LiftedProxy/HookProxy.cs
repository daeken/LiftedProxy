using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Transactions;
using ProxyCore;
using static System.Console;

namespace LiftedProxy {
	public class HookProxy : HttpProxy {
		public HookProxy(IPAddress listenAddress, ushort port, X509Certificate serverCert = null) : base(listenAddress, port, serverCert) {
		}

		public override async Task<bool> PreConnect(TcpClient client) {
			return await Task.FromResult(true);
		}

		public override async Task<HttpRequest> PreRequest(HttpRequest request) {
			return await Task.FromResult(request);
		}

		public override async Task<(string Host, ushort Port)> PreResolve((string Host, ushort Port) target, HttpRequest request) {
			return await Task.FromResult(target);
		}

		public override async Task PostRequest(HttpRequest request, bool success) {
			await Task.FromResult(request);
		}

		public override async Task<HttpResponse> PreResponse(HttpResponse response, HttpRequest request) {
			return await Task.FromResult(response);
		}

		public override async Task PostResponse(HttpResponse response, HttpRequest request) {
			WriteLine($"Persisted id {Persistence.Insert(new Request() {HttpRequest = request, HttpResponse = response})} to disk");
			await Task.FromResult(response);
		}
	}
}