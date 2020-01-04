using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;
using Rollbar;
using Rollbar.NetCore.AspNet;
using Serilog;

namespace RawPotionServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ConfigureRollbarSingleton();

            services.AddRollbarLogger(loggerOptions =>
            {
                loggerOptions.Filter = (loggerName, loglevel) => loglevel >= LogLevel.Trace;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSerilogRequestLogging();

            app.UseMetricServer();
            app.UseRollbarMiddleware();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        /// <summary>
        /// Configures the Rollbar singleton-like notifier.
        /// </summary>
        public void ConfigureRollbarSingleton()
        {
            const string rollbarAccessToken = "206579ccbd5e4cca85b1a44fb18297f6";

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? throw new ArgumentNullException("ASPNETCORE_ENVIRONMENT was not set");

            string rollbarEnvironment;

            if (env.ToLower().Equals("production"))
                rollbarEnvironment = "Production";
            else
                return;

            RollbarConfig rollbarConfig =
                new RollbarConfig(rollbarAccessToken) { Environment = rollbarEnvironment };
            rollbarConfig.ScrubFields = new string[] // data fields which values we want to mask as "***"
            {
                //"url",
                //"method",
            };

            RollbarLocator.RollbarInstance
                // minimally required Rollbar configuration:
                // if you remove line below the logger's configuration will be auto-loaded from appsettings.json
                //.Configure(new RollbarConfig(rollbarAccessToken) { Environment = rollbarEnvironment })
                .Configure(rollbarConfig)
                // optional step if you would like to monitor Rollbar internal events within your application:
                .InternalEvent += OnRollbarInternalEvent;
        }

        /// <summary>
        /// Called when Rollbar internal event detected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RollbarEventArgs"/> instance containing the event data.</param>
        public static void OnRollbarInternalEvent(object sender, RollbarEventArgs e)
        {
            Console.WriteLine(e.TraceAsString());

            RollbarApiErrorEventArgs apiErrorEvent = e as RollbarApiErrorEventArgs;
            if (apiErrorEvent != null)
            {
                //TODO: handle/report Rollbar API communication error event...
                return;
            }
            CommunicationEventArgs commEvent = e as CommunicationEventArgs;
            if (commEvent != null)
            {
                //TODO: handle/report Rollbar API communication event...
                return;
            }
            CommunicationErrorEventArgs commErrorEvent = e as CommunicationErrorEventArgs;
            if (commErrorEvent != null)
            {
                //TODO: handle/report basic communication error while attempting to reach Rollbar API service... 
                return;
            }
            InternalErrorEventArgs internalErrorEvent = e as InternalErrorEventArgs;
            if (internalErrorEvent != null)
            {
                //TODO: handle/report basic internal error while using the Rollbar Notifier... 
                return;
            }
        }

    }
}