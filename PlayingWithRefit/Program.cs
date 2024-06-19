using Microsoft.Extensions.Http.Resilience;
using PlayingWithRefit.Refit;
using PlayingWithRefit.Services;
using Polly;
using Refit;

namespace PlayingWithRefit;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        WaitAndRetryConfig wrc = builder.Configuration.BindTo<WaitAndRetryConfig>();

        // Add services to the container
        {
            services.AddControllers();

            services.AddTransient<AuthorizationMessageHandler>();

            // AuthorizationMessageHandler is a better way to set the token because it has access to DI
            //var refitSettings = new RefitSettings
            //{
            //    AuthorizationHeaderValueGetter = (httpReqMessage, ct) => Task.FromResult("TestToken")
            //};

            // --> Add: RefitClient
            services.AddRefitClient<IUserClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5000"))
                .AddHttpMessageHandler<AuthorizationMessageHandler>()
                //.AddStandardResilienceHandler()
                .AddResilienceHandler("user-pipeline", builder => configureResilienceHandler(builder, wrc));

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

    private static void configureResilienceHandler(ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder, WaitAndRetryConfig wrc)
    {
        // --> Define option: Retry
        var retryOptions = new HttpRetryStrategyOptions
        {
            // ShouldHandle  = ... this is set by default in HttpRetryStrategyOptions
            MaxRetryAttempts = wrc.MaxRetryAttempts,
            Delay            = TimeSpan.FromMilliseconds(wrc.Delay),
            BackoffType      = DelayBackoffType.Constant,
        };

        // --> Configure: Pipeline
        pipelineBuilder
            .AddTimeout(TimeSpan.FromMilliseconds(wrc.TotalRequestTimeout))
            .AddRetry(retryOptions)
            .AddTimeout(TimeSpan.FromMilliseconds(wrc.AttemptTimeout));
    }
}
