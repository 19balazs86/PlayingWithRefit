using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlayingWithRefit.Model;
using PlayingWithRefit.Refit;
using PlayingWithRefit.Services;
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
      _userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
    }

    // This method initiate a call to the UserController with the IUserClient.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> Get(CancellationToken ct)
    {
      Log.Debug("RefitTestController: Start the call.");

      try
      {
        return Ok(await _userClient.GetUsersAsync(ct));
      }
      catch (UserClientException ex)
      {
        return new ContentResult { StatusCode = 500, Content = ex.Message };
      }
      catch (OperationCanceledException)
      {
        Log.Debug("RefitTestController: The operation was canceled.");

        return NoContent();
      }
    }
  }
}