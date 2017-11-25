using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ProxyCore {
	public class HttpProxy {
		public virtual async Task<bool> PreConnect(TcpClient client) => await Task.FromResult(true);
		public virtual async Task<HttpRequest> PreRequest(HttpRequest request) => await Task.FromResult(request);
		public virtual async Task<Tuple<string, ushort>> PreResolve(Tuple<string, ushort> host, HttpRequest request) => await Task.FromResult(host);
		public virtual async Task PostRequest(HttpRequest request, bool success) => await Task.FromResult(request);
		public virtual async Task<HttpResponse> PreResponse(HttpResponse response, HttpRequest request) => await Task.FromResult(response);
		public virtual async Task PostResponse(HttpResponse response, HttpRequest request) => await Task.FromResult(response);

		readonly X509Certificate _serverCertificate;
		readonly IPAddress _listenAddress;
		readonly ushort _port;
		
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
					Task.Factory.StartNewSafeBackground(async () => await ClientLoop(client), TaskCreationOptions.LongRunning);
				}
			}, TaskCreationOptions.LongRunning);
		}

		async Task ClientLoop(TcpClient client) {
			WriteLine($"Accepted client connection from {client.Client.RemoteEndPoint} on {_listenAddress}:{_port}");
			var cstream = client.GetStream();
			while(true) {
				var req = await HttpRequest.Read(cstream);
				WriteLine(req);
				if(req == null)
					break;
				req = await PreRequest(req);
				if(req == null)
					continue;
				
				var resp = await SendReceiveRequest(req);
				await PostRequest(req, resp != null);
				WriteLine(resp);
				if(resp == null)
					return;
				resp = await PreResponse(resp, req);
				if(resp == null)
					return;

				try {
					await cstream.WriteAsync(resp.HeaderText);
					if(resp.Body != null && resp.Body.Length > 0)
						await cstream.WriteAsync(resp.Body);
				} catch(IOException) {
					break;
				}

				await PostResponse(resp, req);
			}
			WriteLine($"Connection from {client.Client.RemoteEndPoint} terminated");
		}

		async Task<HttpResponse> SendReceiveRequest(HttpRequest req) {
			var host = await PreResolve(req.TargetHost, req);
			var conn = new TcpClient(host.Item1, host.Item2);
			var cstream = conn.GetStream();
			await cstream.WriteAsync(req.HeaderText);
			if(req.Body != null && req.Body.Length != 0)
				await cstream.WriteAsync(req.Body, 0, req.Body.Length);

			var resp = await HttpResponse.Read(cstream);
			return resp;
		}
	}
}