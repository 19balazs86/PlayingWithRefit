using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlayingWithRefit.Model;
using PlayingWithRefit.Refit;
using Refit;
using Serilog;

namespace PlayingWithRefit.Controllers
{
  [Route("json-placeholder")]
  [ApiController]
  public class JsonPlaceholderController : ControllerBase
  {
    private readonly IJsonPlaceholderClient _jpClient;

    public JsonPlaceholderController(IJsonPlaceholderClient jpClient)
    {
      _jpClient = jpClient ?? throw new ArgumentNullException(nameof(jpClient));
    }

    [HttpGet("posts")]
    public Task<ActionResult<IEnumerable<Post>>> GetAllPost()
      => callClientMethod(_jpClient.GetAllPostAsync, nameof(_jpClient.GetAllPostAsync));

    // Try this id > 100
    [HttpGet("posts/{id}")]
    public Task<ActionResult<Post>> GetPost(int id)
      => callClientMethod(() => _jpClient.GetPostAsync(id), nameof(_jpClient.GetPostAsync));

    [HttpPost("posts")]
    public Task<ActionResult<Post>> CreatePost(Post post)
      => callClientMethod(() => _jpClient.CreatePostAsync(post), nameof(_jpClient.CreatePostAsync));

    // Try this id > 100
    [HttpPut("posts/{id}")]
    public Task<ActionResult<Post>> GetPost(int id, Post post)
      => callClientMethod(() => _jpClient.UpdatePostAsync(id, post), nameof(_jpClient.UpdatePostAsync));

    [HttpDelete("posts/{id}")]
    public Task<IActionResult> DeletePost(int id)
      => callClientMethod(() => _jpClient.DeletePostAsync(id), nameof(_jpClient.DeletePostAsync));


    private async Task<ActionResult<T>> callClientMethod<T>(Func<Task<T>> func, string methodName)
    {
      try
      {
        return Ok(await func());
      }
      catch (ApiException ex)
      {
        Log.Error(ex, $"ApiException by calling the '{methodName}' method.");

        IDictionary<string, object> result = ex.ToDictionary();
        result.Add("MethodName", methodName);

        return new JsonResult(result) { StatusCode = 500 };
      }
      catch (Exception ex)
      {
        Log.Error(ex, $"Error by calling the '{methodName}' method.");

        return new ContentResult { StatusCode = 500, Content = $"Message: '{ex.Message}'" };
      }
    }

    private async Task<IActionResult> callClientMethod(Func<Task> func, string methodName)
    {
      try
      {
        await func();

        return Ok();
      }
      catch (ApiException ex)
      {
        Log.Error(ex, $"ApiException by calling the '{methodName}' method.");

        IDictionary<string, object> result = ex.ToDictionary();
        result.Add("MethodName", methodName);

        return new JsonResult(result) { StatusCode = 500 };
      }
      catch (Exception ex)
      {
        Log.Error(ex, $"Error by calling the '{methodName}' method.");

        return new ContentResult { StatusCode = 500, Content = $"Message: '{ex.Message}'" };
      }
    }
  }
}