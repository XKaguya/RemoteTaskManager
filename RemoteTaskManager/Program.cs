using System;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RemoteTaskManager.Web;

namespace RemoteTaskManager
{
    public class Program
    {
        private static CancellationTokenSource? WebCancellactionTokenSource = new();
        public static ushort WebPort { get; set; } = 8181;
        
        public static async Task Main(string[] args)
        {
            InitWebServer();
            Console.WriteLine("Task WebService started.");
            Console.WriteLine("Press any key to stop.");
            Console.ReadLine();
            WebCancellactionTokenSource.Cancel();
            Console.WriteLine("Task WebService stopped.");
            
            Environment.Exit(0);
        }
        
        private static async Task InitWebServer()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var host = new WebHostBuilder()
                        .UseKestrel(options => { options.Listen(IPAddress.Any, WebPort); })
                        .ConfigureServices(services =>
                        {
                            var webService = new Startup();
                            webService.ConfigureServices(services);
                        })
                        .Configure(app =>
                        {
                            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                            var webService = new Startup();
                            webService.Configure(app, env);
                        })
                        .Build();

                    await host.RunAsync(WebCancellactionTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"Task WebService stopped.");
                }
            });
        }

    }
}