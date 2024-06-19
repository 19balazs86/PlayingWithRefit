using PlayingWithRefit.Model;
using PlayingWithRefit.Refit;
using Polly.Timeout;
using Refit;

namespace PlayingWithRefit.Services;

/// <summary>
/// This is a decorator/wrapper class for IUserClient added as RefitClient.
/// </summary>
public sealed class UserClient : IUserClient
{
    private readonly IUserClient _userClient;

    public UserClient(IUserClient userClient) => _userClient = userClient;

    public async Task<IEnumerable<UserDto>> GetUsersAsync(CancellationToken ct)
    {
        try
        {
            return await _userClient.GetUsersAsync(ct);
        }
        //catch (ValidationApiException ex) { }
        catch (ApiException ex)
        {
            // ApiException: When the response has a failed status code (4xx, 5xx).

            throw new UserClientException(ex);
        }
        catch (TimeoutRejectedException ex) // ExecutionRejectedException can be use to catch Polly's exceptions: https://www.pollydocs.org/api/Polly.ExecutionRejectedException.html
        {
            throw new UserClientException(ex.Message, ex);
        }
    }
}
