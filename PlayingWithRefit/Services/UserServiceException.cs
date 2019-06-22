using System;
using System.Runtime.Serialization;
using Refit;

namespace PlayingWithRefit.Services
{
  public class UserServiceException : Exception
  {
    public UserServiceException()
    {
    }

    public UserServiceException(string message) : base(message)
    {
    }

    public UserServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected UserServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public UserServiceException(ApiException ex) : base($"{ex.Message} @{ex.HttpMethod.Method}('{ex.Uri.AbsoluteUri}') Content: '{ex.Content}'", ex)
    {
    }
  }
}
