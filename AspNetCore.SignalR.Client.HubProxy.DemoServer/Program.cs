using AspNetCore.SignalR.Client.HubProxy.DemoCommon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AspNetCore.SignalR.Client.HubProxy.DemoServer
{

    public class HubComms
    {

    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var hubsComms = new HubComms();


            var host = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton(hubsComms);
                })
                .UseSetting(WebHostDefaults.PreventHostingStartupKey, "true")
                .ConfigureLogging(logging =>
                {
                    logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
                    //logging.AddProvider(new SignalRLogger());
                    //logging.AddDebug();
                })
                .UseKestrel(options =>
                {
                    options.ListenAnyIP(1984);
                })
                .UseStartup<Startup>()
                .Build();

            var hubContext = host.Services.GetService<IHubContext<ServerHub>>();

            hostRun = host.RunAsync();

            

            await hostRun;
        }
        static Task hostRun;

        public class Startup
        {
            public IConfiguration Configuration { get; }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddSignalR(options =>
                {
                    options.EnableDetailedErrors = true;
                });

                services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
                });
            }

            public void Configure(IApplicationBuilder app)
            {
                app.UseCors("CorsPolicy");
                app.UseSignalR(routes =>
                {
                    routes.MapHub<ServerHub>("/serverhub", options => options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets);
                });
            }
        }
    }
}
