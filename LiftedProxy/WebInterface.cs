using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Nancy;
using Nancy.Configuration;
using Nancy.Owin;
using static System.Console;

namespace LiftedProxy {
	public class IndexModule : NancyModule {
		public IndexModule() {
			Get("/", _ => "Test");
		}
	}
	
	public class Startup {
		public void Configure(IApplicationBuilder app) {
			app.UseDeveloperExceptionPage();
			app.UseOwin(x => x.UseNancy());
		}
	}

	public class Bootstrapper : DefaultNancyBootstrapper {
		public override void Configure(INancyEnvironment environment) {
			environment.Tracing(enabled: false, displayErrorTraces: true);
		}
	}
	
	public class WebInterface {
		public WebInterface() {
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