using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Nancy;
using Nancy.Configuration;
using Nancy.Conventions;
using Nancy.Owin;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using static System.Console;

namespace LiftedProxy {
	public class IndexModule : NancyModule {
		public IndexModule() {
			Get("/", _ => Response.AsFile("Client/bin/Debug/bridge/index.html"));
			Get("/api/proxyHistory", _ => Response.AsJson(Persistence.GetRequests()));
		}
	}
	
	public class Startup {
		public void Configure(IApplicationBuilder app) {
			app.UseDeveloperExceptionPage();
			app.UseOwin(x => x.UseNancy());
		}
	}

	public class Bootstrapper : DefaultNancyBootstrapper {
		public class SelfHostRootPathProvider : IRootPathProvider {
			public string GetRootPath() => Directory.GetCurrentDirectory() + "/../";	
		}
		
		public override void Configure(INancyEnvironment environment) {
			environment.Tracing(enabled: false, displayErrorTraces: true);
		}

		protected override void ConfigureConventions(NancyConventions nancyConventions) {
			base.ConfigureConventions(nancyConventions);
			nancyConventions.StaticContentsConventions.AddDirectory("/", "./Client/bin/Debug/bridge/");
		}

		protected override IRootPathProvider RootPathProvider => new SelfHostRootPathProvider();
	}
	
	public static class WebInterface {
		public static void Setup() {
			var config = new ConfigurationBuilder().AddCommandLine(new [] {"--environment", "development"}).Build();
			var host = new WebHostBuilder()
				.UseConfiguration(config)
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}
	}
}