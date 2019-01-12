using System.Collections.Generic;
using Refit;

namespace PlayingWithRefit.Refit
{
  public static class ApiExceptionExtensions
  {
    public static IDictionary<string, object> ToDictionary(this ApiException apiException)
    {
      if (apiException is null)
        return new Dictionary<string, object>();

      IDictionary<string, object> fieldDictionary = new Dictionary<string, object>
      {
        ["Url"]        = apiException.Uri.AbsoluteUri,
        ["Method"]     = apiException.HttpMethod.Method,
        ["StatusCode"] = apiException.StatusCode,
        ["Reason"]     = apiException.ReasonPhrase,
        ["Content"]    = apiException.Content,
      };

      return fieldDictionary;
    }
  }
}
