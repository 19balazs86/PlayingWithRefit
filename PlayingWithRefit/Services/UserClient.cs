using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayingWithRefit.Model;
using PlayingWithRefit.Refit;
using Polly.Timeout;
using Refit;

namespace PlayingWithRefit.Services
{
  /// <summary>
  /// This is a decorator/wrapper class for IUserClient added as RefitClient.
  /// </summary>
  public class UserClient : IUserClient
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

        throw new UserServiceException(ex);
      }
      catch (Exception ex) when (ex is TimeoutRejectedException || ex is JsonReaderException)
      {
        // TimeoutRejectedException: Thrown by Polly TimeoutPolicy.
        // JsonReaderException: When you have a problem to deserialize the response.

        throw new UserServiceException(ex.Message, ex);
      }
    }
  }
}
