using Microsoft.AspNetCore.Mvc;
using PlayingWithRefit.Model;
using PlayingWithRefit.Refit;
using PlayingWithRefit.Services;

namespace PlayingWithRefit.Controllers;

[Route("test")]
[ApiController]
public sealed class RefitTestController : ControllerBase
{
    private readonly ILogger<RefitTestController> _logger;

    private readonly IUserClient _userClient;

    public RefitTestController(ILogger<RefitTestController> logger, IUserClient userClient)
    {
        ArgumentNullException.ThrowIfNull(userClient, nameof(userClient));

        _logger     = logger;
        _userClient = userClient;
    }

    // This method initiate a call to the UserController with the IUserClient.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> Get(CancellationToken ct)
    {
        _logger.LogDebug("RefitTestController: Start the call.");

        try
        {
            return Ok(await _userClient.GetUsersAsync(ct));
        }
        catch (UserClientException ex)
        {
            return new ContentResult { StatusCode = 500, Content = $"Message: '{ex.Message}'" };
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("RefitTestController: The operation was canceled.");

            return NoContent();
        }
    }
}