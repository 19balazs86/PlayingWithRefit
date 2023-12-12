# Playing with Refit

This small WebAPI is an example of using Refit, an automatic type-safe REST library. This concept can be beneficial for initiating calls to 3rd-party services.

#### Resources

- [Refit](https://reactiveui.github.io/refit) 👤*Official*
- [Refitter](https://github.com/christianhelle/refitter) 👤*Christian Helle* - Generate the Refit interface from OpenAPI specifications
- [Logging HTTP Traffic](https://blog.nimblepros.com/blogs/refit-http-request-response-logging) 📓*NimblePros* - Using HttpMessageHandler

#### Polly and Refit hand in hand

- With Refit, you can use [Polly](https://github.com/App-vNext/Polly) as a resilience and transient-fault-handling library, which can helps you to easily write [retry logic](https://learn.microsoft.com/en-ie/aspnet/core/fundamentals/http-requests?view=aspnetcore-7.0#use-polly-based-handlers).
- [Using Polly with HttpClient factory](https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory) 👤
- In the example, I use a timeout policy to cancel a long-running call. You can find a solution for using `CancellationToken` in case the client-side application cancels the request.

###### Alternatives

- Popular library: [RestSharp](https://restsharp.dev) | [RestSharp examples](https://jasonwatmore.com/c-restsharp-http-post-request-examples-in-net) 📓*Jason Watmore*
- [DalSoft.RestClient](https://code-maze.com/dalsoft-restclient-consume-any-rest-api) 📓*CodeMaze - Dynamic and fluent C# rest client library*

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
