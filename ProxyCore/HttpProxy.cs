using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static System.Console;

namespace ProxyCore {
	public class HttpProxy {
		public virtual async Task<bool> PreConnect(TcpClient client) => await Task.FromResult(true);
		public virtual async Task<string> PreResolve(string hostname) => await Task.FromResult(hostname);
		public virtual async Task<HttpRequest> PreRequest(HttpRequest request) => await Task.FromResult(request);
		public virtual async Task<HttpRequest> PostRequest(HttpRequest request) => await Task.FromResult(request);
		public virtual async Task<HttpResponse> PreResponse(HttpResponse response) => await Task.FromResult(response);
		public virtual async Task<HttpResponse> PostResponse(HttpResponse response) => await Task.FromResult(response);

		X509Certificate _serverCertificate;
		IPAddress _listenAddress;
		ushort _port;
		
		public HttpProxy(IPAddress listenAddress, ushort port, X509Certificate serverCert = null) {
			_listenAddress = listenAddress;
			_port = port;
			_serverCertificate = serverCert;
			var listener = new TcpListener(listenAddress, port);
			listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
			listener.Start();
			WriteLine($"Proxy listening on {listenAddress}:{port}");
			Task.Factory.StartNew(async () => {
				while(true) {
					var client = await listener.AcceptTcpClientAsync();
					if(await PreConnect(client) != true)
						return;
					Task.Factory.StartNew(async () => await ClientLoop(client), TaskCreationOptions.LongRunning);
				}
			}, TaskCreationOptions.LongRunning);
		}

		private async Task ClientLoop(TcpClient client) {
			WriteLine($"Accepted client connection from {client.Client.RemoteEndPoint} on {_listenAddress}:{_port}");
			var cstream = client.GetStream();
			while(true) {
				var req = await new HttpRequest().Read(cstream);
				if(req == null)
					break;
			}
			WriteLine($"Connection from {client.Client.RemoteEndPoint} terminated");
		}
	}
}