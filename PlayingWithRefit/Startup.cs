using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayingWithRefit.Refit;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Refit;
using Serilog;
using Serilog.Events;

namespace PlayingWithRefit
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;

      // --> Init: Logger.
      initLogger();
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      WaitAndRetryConfig wrc = Configuration.BindTo<WaitAndRetryConfig>();

      // Add: MessageHandler(s) to the DI container.
      services.AddTransient<AuthorizationMessageHandler>();

      // --> Create: Polly policy.
      Policy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .Or<TimeoutRejectedException>() // Thrown by Polly's TimeoutPolicy if the inner call gets timeout.
        .WaitAndRetryAsync(wrc.Retry, _ => TimeSpan.FromMilliseconds(wrc.Wait));

      Policy<HttpResponseMessage> timeoutPolicy = Policy
        .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(wrc.Timeout));

      // !! Problem: AuthorizationHeaderValueGetter is not called by the library, if you add with AddRefitClient.

      //RefitSettings refitSettings = new RefitSettings
      //{
      //  AuthorizationHeaderValueGetter = () => Task.FromResult("TestToken")
      //};

      // -- Add: RefitClient.
      services.AddRefitClient<IUserClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5000"))
        .AddPolicyHandler(retryPolicy)
        .AddPolicyHandler(timeoutPolicy) // The order of adding is imporant!
        .AddHttpMessageHandler<AuthorizationMessageHandler>(); // RefitSettings does not work.

      services.AddRefitClient<IJsonPlaceholderClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://jsonplaceholder.typicode.com"));
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseDeveloperExceptionPage();

      app.UseMvc();
    }

    private static void initLogger()
    {
      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Information) // Gives you useful information, but not necessary.
        //.Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message}{NewLine}{Exception}")
        .CreateLogger();
    }
  }
}
