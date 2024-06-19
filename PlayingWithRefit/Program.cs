using PlayingWithRefit.Refit;
using PlayingWithRefit.Services;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using Refit;

namespace PlayingWithRefit;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        var services      = builder.Services;
        var configuration = builder.Configuration;

        // Add services to the container
        {
            services.AddControllers();

            WaitAndRetryConfig wrc = configuration.BindTo<WaitAndRetryConfig>();

            services.AddTransient<AuthorizationMessageHandler>();

            // --> Create: Polly policy
            AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>() // Thrown by Polly's TimeoutPolicy if the inner call gets timeout.
                .WaitAndRetryAsync(wrc.Retry, _ => TimeSpan.FromMilliseconds(wrc.Wait));

            AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicy = Policy
                .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(wrc.Timeout));

            // AuthorizationMessageHandler is a better way to set the token because it has access to DI
            //var refitSettings = new RefitSettings
            //{
            //    AuthorizationHeaderValueGetter = (httpReqMessage, ct) => Task.FromResult("TestToken")
            //};

            // --> Add: RefitClient
            services.AddRefitClient<IUserClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5000"))
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutPolicy) // The order of adding is imporant!
                .AddHttpMessageHandler<AuthorizationMessageHandler>();

            // Using Scrutor to automatically register services DI container
            // https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container

            // --> Decorate IUserClient(RefitClient) with UserClient implementation
            services.Decorate<IUserClient, UserClient>();

            services.AddRefitClient<IJsonPlaceholderClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://jsonplaceholder.typicode.com"));
        }

        WebApplication app = builder.Build();

        // Configure the request pipeline
        {
            app.MapControllers();
        }

        app.Run();
    }
}
