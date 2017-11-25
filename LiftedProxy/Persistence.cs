using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using ProxyCore;

namespace LiftedProxy {
	public class Request {
		public ObjectId Id { get; set; }
		public HttpRequest HttpRequest { get; set; }
		public HttpResponse HttpResponse { get; set; }
	}
	public class Persistence {
		public static readonly Persistence Instance = new Persistence();
		
		readonly LiteCollection<Request> _requests;
		public Persistence() {
			var db = new LiteDatabase(@"Filename=store.db; Mode=exclusive");
			_requests = db.GetCollection<Request>("requests");

			BsonMapper.Global.RegisterType(
				serialize: hd => new BsonValue(hd.Headers.Select(x => new [] { x.Key, x.Value })), 
				deserialize: bson => new HeaderDictionary(bson.AsArray.ToList().Select(x => x.AsArray).Select(x => (x[0].AsString, x[1].AsString)).ToList())
			);
		}

		public static ObjectId Insert(Request req) {
			Instance._requests.Insert(req);
			return req.Id;
		}

		public static IEnumerable<Request> GetRequests() {
			return Instance._requests.FindAll();
		}
	}
}