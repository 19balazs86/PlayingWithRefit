# Playing with Refit

- This small WebAPI is an example of using Refit, an automatic type-safe REST library. This concept can be beneficial for initiating calls to 3rd-party services
- With Refit, you can use **Polly** as a resilience and transient-fault-handling library

## Resources

- [Refit](https://reactiveui.github.io/refit) 👤*Official*
- [Working with Refit type-safe REST library](https://www.milanjovanovic.tech/blog/refit-in-dotnet-building-robust-api-clients-in-csharp) 📓*Milan newsletter*
- [Refitter](https://github.com/christianhelle/refitter) 👤*Christian Helle - Generate the Refit interface from OpenAPI specifications*
- [Logging HTTP Traffic](https://blog.nimblepros.com/blogs/refit-http-request-response-logging) 📓*NimblePros - Using HttpMessageHandler*
- [Bearer Authentication](https://blog.nimblepros.com/blogs/refit-bearer-auth) 📓*NimblePros*
- [Playing with HttpClientFactory](https://github.com/19balazs86/PlayingWithHttpClientFactory) 👤*My repository*

## Alternatives

- Popular library: [RestSharp](https://restsharp.dev) | [RestSharp examples](https://jasonwatmore.com/c-restsharp-http-post-request-examples-in-net) 📓*Jason Watmore*
- [DalSoft.RestClient](https://code-maze.com/dalsoft-restclient-consume-any-rest-api) 📓*CodeMaze - Dynamic and fluent C# rest client library*
- [Apizr](https://github.com/Respawnsive/Apizr) 👤*Respawnsive* | [Getting started](https://www.respawnsive.com/starcellar-e01-getting-started-with-apizr) 📓 | *Refit based WebApi client management, but resilient (retry, connectivity, cache, auth, log, priority, etc...)*

## ConfigureServices in action

```csharp
public void ConfigureServices(IServiceCollection services)
{
  services.AddTransient<AuthorizationMessageHandler>();

  services.AddRefitClient<IUserClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5000"))
    .AddHttpMessageHandler<AuthorizationMessageHandler>()
    .AddResilienceHandler("user-pipeline", configureResilienceHandler);
}

private static void configureResilienceHandler(ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder)
{
  // --> Define option: Retry
  var retryOptions = new HttpRetryStrategyOptions
  {
    MaxRetryAttempts = 2,
    Delay            = TimeSpan.FromMilliseconds(500),
    BackoffType      = DelayBackoffType.Constant,
  };

  // --> Configure: Pipeline
  pipelineBuilder
    .AddTimeout(TimeSpan.FromSeconds(5))
    .AddRetry(retryOptions)
    .AddTimeout(TimeSpan.FromMilliseconds(500));
}
```
