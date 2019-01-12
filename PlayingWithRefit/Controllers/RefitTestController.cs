using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlayingWithRefit.Model;
using PlayingWithRefit.Refit;
using Polly.Timeout;
using Refit;
using Serilog;

namespace PlayingWithRefit.Controllers
{
  [Route("test")]
  [ApiController]
  public class RefitTestController : ControllerBase
  {
    private readonly IUserClient _userClient;

    public RefitTestController(IUserClient userClient)
    {
      // Here, you may inject your business logic and not directly the service/client.
      _userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> Get(CancellationToken ct)
    {
      Log.Debug("RefitTestController: Start the call.");

      try
      {
        return Ok(await _userClient.GetUsersAsync(ct));
      }
      //catch (ValidationApiException ex) { }
      catch (ApiException ex)
      {
        // ApiException: When the response has failed status codes (4xx, 5xx).

        var responseContent = new { ex.StatusCode, Content = ex.HasContent ? ex.Content : "NoContent" };

        return new JsonResult(responseContent) { StatusCode = 500 };
      }
      catch (Exception ex) when (ex is TimeoutRejectedException || ex is JsonReaderException)
      {
        // TimeoutRejectedException: Thrown by Polly TimeoutPolicy.
        // JsonReaderException: When you have a problem to deserialize the response.

        return new ContentResult
        {
          StatusCode = 500,
          Content    = $"Message: '{ex.Message}'"
        };
      }
      catch (OperationCanceledException)
      {
        Log.Debug("RefitTestController: The operation was canceled.");

        return NoContent();
      }
    }
  }
}