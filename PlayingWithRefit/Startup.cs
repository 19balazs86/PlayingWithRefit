using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayingWithRefit.Refit;
using PlayingWithRefit.Services;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using Refit;

namespace PlayingWithRefit
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();

      WaitAndRetryConfig wrc = Configuration.BindTo<WaitAndRetryConfig>();

      // Add: MessageHandler(s) to the DI container.
      services.AddTransient<AuthorizationMessageHandler>();

      // --> Create: Polly policy.
      AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .Or<TimeoutRejectedException>() // Thrown by Polly's TimeoutPolicy if the inner call gets timeout.
        .WaitAndRetryAsync(wrc.Retry, _ => TimeSpan.FromMilliseconds(wrc.Wait));

      AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicy = Policy
        .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(wrc.Timeout));

      // !! Problem: AuthorizationHeaderValueGetter is not called by the library, if you add with AddRefitClient.

      //RefitSettings refitSettings = new RefitSettings
      //{
      //  AuthorizationHeaderValueGetter = () => Task.FromResult("TestToken")
      //};

      // --> Add: RefitClient.
      services.AddRefitClient<IUserClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5000"))
        .AddPolicyHandler(retryPolicy)
        .AddPolicyHandler(timeoutPolicy) // The order of adding is imporant!
        .AddHttpMessageHandler<AuthorizationMessageHandler>(); // RefitSettings does not work.

      // --> Decorate IUserClient(RefitClient) with UserClient implementation.
      services.Decorate<IUserClient, UserClient>();

      services.AddRefitClient<IJsonPlaceholderClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://jsonplaceholder.typicode.com"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseDeveloperExceptionPage();

      app.UseRouting();

      app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
  }
}
