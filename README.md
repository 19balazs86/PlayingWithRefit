# Playing with Refit

This small ASP.NET Core WebAPI is an example to use [Refit](https://reactiveui.github.io/refit "Refit"), automatic type-safe REST library.

This concept can be useful to initiate 3rd party services calls. Refit can help you to make it easier. 

In my [Playing with HttpClientFactory](https://github.com/19balazs86/PlayingWithHttpClientFactory "Playing with HttpClientFactory"), I used the built-in HttpClientFactory to initiate http calls.

With Refit, you can use [Polly](https://github.com/App-vNext/Polly "Polly") as a resilience and transient-fault-handling library, which can helps you to easily write [retry logic](https://docs.microsoft.com/en-ie/aspnet/core/fundamentals/http-requests?view=aspnetcore-2.2#use-polly-based-handlers "retry logic").
Other useful information: [Polly and HttpClientFactory](https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory "Polly and HttpClientFactory").

In the example, I use a timeout policy to cancel a long running call. You can find a solution to use CancellationToken in case, if the client side application cancel the request.

Scott Hanselman: [Exploring refit, an automatic type-safe REST library for .NET Standard](https://www.hanselman.com/blog/ExploringRefitAnAutomaticTypesafeRESTLibraryForNETStandard.aspx "Exploring refit, an automatic type-safe REST library for .NET Standard").

*ConfigureServices in action:*

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add: MessageHandler(s) to the DI container.
    services.AddTransient<AuthorizationMessageHandler>();

    // --> Create: Polly policy.
    Policy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .Or<TimeoutRejectedException>() // Thrown by Polly's TimeoutPolicy if the inner call gets timeout.
        .WaitAndRetryAsync(wrc.Retry, _ => TimeSpan.FromMilliseconds(wrc.Wait));

    Policy<HttpResponseMessage> timeoutPolicy = Policy
        .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(wrc.Timeout));
    
    // -- Add: RefitClient.
    services.AddRefitClient<IUserClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5000"))
        .AddPolicyHandler(retryPolicy)
        .AddPolicyHandler(timeoutPolicy) // The order of adding is imporant!
        .AddHttpMessageHandler<AuthorizationMessageHandler>();
}
```