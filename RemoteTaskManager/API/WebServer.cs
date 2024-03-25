using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using RemoteTaskManager.API;
using RemoteTaskManager.Web.Controllers;

namespace RemoteTaskManager.Web
{
    public class Startup
    {
        // This is ASP.NET Core Web Class.
        // It will be responsible for handling HTTP requests and returning responses to replace the WebSocketServer.
        // It is a local server that no need for authentication or encryption.
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
            services.AddControllers()
                .AddApplicationPart(typeof(ProcessController).Assembly);
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.Run(async (context) =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "RemoteTaskManager.Web.Index.html";
                    

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();

                    context.Response.ContentType = "text/html";

                    await context.Response.WriteAsync(result);
                }
            });
        }
    }
}

namespace RemoteTaskManager.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {
        [HttpGet("Test")]
        public IActionResult Get()
        {
            return Ok("Hello World!");
        }

        [HttpGet("GetProcessInfo")]
        public async Task<IActionResult> GetProcessInfo()
        {
            string processInfo = await Handler.GetProcessInfo();
            return Ok(processInfo);
        }
        
        [HttpPost("KillProcess")]
        public async Task<IActionResult> KillProcess()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                try
                {
                    // This method should accept a JSON object with the process name and supermacy mode.
                    // It should then try to kill the process with the given ID and the supermacy mode.
                    string json = await reader.ReadToEndAsync();
                    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    int pid = data.Pid;
                    bool supermacyMode = data.supermacyMode;
                    
                    bool success = Handler.KillProcess(pid, supermacyMode);
                    return Ok(success);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
    }
}