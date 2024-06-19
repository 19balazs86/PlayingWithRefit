using Refit;

namespace PlayingWithRefit.Services;

public sealed class UserClientException : Exception
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

    public UserClientException(ApiException ex) : base($"{ex.Message} @{ex.HttpMethod.Method}('{ex.Uri.AbsoluteUri}') Content: '{ex.Content}'", ex)
    {
    }
}
