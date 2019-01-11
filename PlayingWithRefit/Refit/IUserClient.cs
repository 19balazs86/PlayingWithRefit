using System.Collections.Generic;
using System.Threading.Tasks;
using PlayingWithRefit.Model;
using Refit;

namespace PlayingWithRefit.Refit
{
  [Headers("Authorization: Bearer")]
  public interface IUserClient
  {
    [Get("/User")]
    //[Headers("Authorization: Bearer")]
    Task<IEnumerable<UserDto>> GetUsersAsync();
  }
}
