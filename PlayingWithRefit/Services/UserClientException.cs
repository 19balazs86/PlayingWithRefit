using Refit;
using System.Runtime.Serialization;

namespace PlayingWithRefit.Services;

public class UserClientException : Exception
{
    public UserClientException()
    {
    }

    public UserClientException(string message) : base(message)
    {
    }

    public UserClientException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected UserClientException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public UserClientException(ApiException ex) : base($"{ex.Message} @{ex.HttpMethod.Method}('{ex.Uri.AbsoluteUri}') Content: '{ex.Content}'", ex)
    {
    }
}
