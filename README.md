# Playing with Refit

This small ASP.NET Core WebAPI is an example to use [Refit](https://reactiveui.github.io/refit), automatic type-safe REST library.

This concept can be useful to initiate 3rd party services calls. Refit can help you to make it easier. 

#### Polly and Refit hand in hand

- With Refit, you can use [Polly](https://github.com/App-vNext/Polly) as a resilience and transient-fault-handling library, which can helps you to easily write [retry logic](https://docs.microsoft.com/en-ie/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.0#use-polly-based-handlers).
- Other useful information: [Polly and HttpClientFactory](https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory).
- In the example, I use a timeout policy to cancel a long running call. You can find a solution to use `CancellationToken` in case, if the client side application cancel the request.

#### Resources

- Scott Hanselman: [Exploring refit, an automatic type-safe REST library for .NET Standard](https://www.hanselman.com/blog/ExploringRefitAnAutomaticTypesafeRESTLibraryForNETStandard.aspx).
- Code Maze blog: [DalSoft.RestClient](https://code-maze.com/dalsoft-restclient-consume-any-rest-api), dynamic and fluent C# rest client library.
- Xamarin Show: [Simplify HTTP in Mobile Apps with Refit](https://www.youtube.com/watch?v=IUP0XFs6XRI).

> In my [Playing with HttpClientFactory](https://github.com/19balazs86/PlayingWithHttpClientFactory), I used the built-in `HttpClientFactory` to initiate http calls.

#### ConfigureServices in action

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add: MessageHandler(s) to the DI container.
    services.AddTransient<AuthorizationMessageHandler>();

    // --> Create: Polly policy.
    AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .Or<TimeoutRejectedException>() // Thrown by Polly's TimeoutPolicy if the inner call gets timeout.
        .WaitAndRetryAsync(wrc.Retry, _ => TimeSpan.FromMilliseconds(wrc.Wait));

    AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicy = Policy
        .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(wrc.Timeout));
    
    // -- Add: RefitClient.
    services.AddRefitClient<IUserClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5000"))
        .AddPolicyHandler(retryPolicy)
        .AddPolicyHandler(timeoutPolicy) // The order of adding is imporant!
        .AddHttpMessageHandler<AuthorizationMessageHandler>();
}
```
