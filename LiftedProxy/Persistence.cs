using System;
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
		public static Persistence Instance = new Persistence();
		
		LiteDatabase _db;
		LiteCollection<Request> _requests;
		public Persistence() {
			_db = new LiteDatabase(@"Filename=store.db; Mode=exclusive");
			_requests = _db.GetCollection<Request>("requests");

			BsonMapper.Global.RegisterType<HeaderDictionary>(
				serialize: hd => new BsonValue(hd.Headers.Select(x => new [] { x.Item1, x.Item2 })), 
				deserialize: bson => new HeaderDictionary(bson.AsArray.ToList().Select(x => x.AsArray).Select(x => Tuple.Create(x[0].AsString, x[1].AsString)).ToList())
			);
		}

		public static ObjectId Insert(Request req) {
			Instance._requests.Insert(req);
			return req.Id;
		}
	}
}