using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rollbar;
using Rollbar.PlugIns.Serilog;
using Serilog;
using Serilog.Events;

namespace RawPotionServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Define RollbarConfig:

            const string rollbarAccessToken = "206579ccbd5e4cca85b1a44fb18297f6";

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                      throw new ArgumentNullException("ASPNETCORE_ENVIRONMENT was not set");

            string rollbarEnvironment;

            if (env.ToLower().Equals("production"))
                rollbarEnvironment = "Production";
            else
                rollbarEnvironment = "Development";

            RollbarConfig rollbarConfig = new RollbarConfig(rollbarAccessToken)
            {
                Environment = rollbarEnvironment,
            };

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollbarSink(rollbarConfig)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var port = Environment.GetEnvironmentVariable("PORT");

                    if (!string.IsNullOrEmpty(port))
                    {
                        var urls = new string[]
                        {
                            "http://*:" + port
                        };

                        webBuilder.UseUrls(urls);
                    }

                    webBuilder.UseStartup<Startup>().UseSerilog();
                });
    }
}