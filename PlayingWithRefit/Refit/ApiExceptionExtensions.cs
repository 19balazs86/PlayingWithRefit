using System.Collections.Generic;
using Refit;

namespace PlayingWithRefit.Refit
{
  public static class ApiExceptionExtensions
  {
    // Note: Technically, this is a IDictionary extension...
    public static void AddApiExceptionFields(this IDictionary<string, object> dic, ApiException exception)
    {
      if (dic is null)
        dic = new Dictionary<string, object>();

      if (exception is null) return;

      dic.TryAdd("Url", exception.Uri.AbsoluteUri);
      dic.TryAdd("Method", exception.HttpMethod.Method);
      dic.TryAdd("StatusCode", exception.StatusCode);
      dic.TryAdd("Message", exception.Message);
      dic.TryAdd("Content", exception.Content);
    }
  }
}
