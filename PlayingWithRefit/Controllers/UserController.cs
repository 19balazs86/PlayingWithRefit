using Microsoft.AspNetCore.Mvc;
using PlayingWithRefit.Model;
using System.Net;

namespace PlayingWithRefit.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    private readonly HttpStatusCode[] _httpStatusCodes = new HttpStatusCode[]
    {
        HttpStatusCode.BadRequest,  // Polly won't retry for this.
        HttpStatusCode.NotFound,    // Polly won't retry for this.
        HttpStatusCode.RequestTimeout,
        HttpStatusCode.RequestTimeout,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.OK, HttpStatusCode.OK, HttpStatusCode.OK,
        HttpStatusCode.OK, HttpStatusCode.OK, HttpStatusCode.OK,
    };

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    // This method is called by the RefitTestController.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> Get(CancellationToken ct)
    {
        // This should be "Bearer TestToken". IUserClient interface have an Authorization attribute.
        //StringValues authorizationToken;
        //HttpContext.Request.Headers.TryGetValue("Authorization", out authorizationToken);

        HttpStatusCode selectedStatusCode = _httpStatusCodes[Random.Shared.Next(_httpStatusCodes.Length)];

        _logger.LogDebug("UserController: Selected status code: {selectedStatusCode}", selectedStatusCode);

        // --> Return OK.
        if (selectedStatusCode == HttpStatusCode.OK)
            return Ok(Enumerable.Range(1, 5).Select(x => new UserDto { Id = x, Name = $"Name #{x}" }));

        // --> Delay.
        if (selectedStatusCode == HttpStatusCode.RequestTimeout)
        {
            try
            {
                // If your method do not accept token in the argument, you can check it here beforehand.
                ct.ThrowIfCancellationRequested();

                await Task.Delay(5000, ct);
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("UserController: The operation was canceled.");

                return NoContent();
            }

            // The timeout policy will end this call earlier, so you won't see this line.
            _logger.LogDebug($"UserController: After the delay.");
        }

        // --> Other returns.
        return new ContentResult
        {
            StatusCode = (int)selectedStatusCode,
            Content    = $"Selected status code: {selectedStatusCode}"
        };
    }
}
