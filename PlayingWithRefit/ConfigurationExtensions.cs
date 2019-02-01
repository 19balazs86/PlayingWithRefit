using Microsoft.Extensions.Configuration;

namespace PlayingWithRefit
{
  public static class ConfigurationExtensions
  {
    public static T BindTo<T>(this IConfiguration configuration, string key) where T : class, new()
    {
      T bindingObject = new T();

      configuration.GetSection(key).Bind(bindingObject);

      return bindingObject;
    }

    public static T BindTo<T>(this IConfiguration configuration) where T : class, new()
      => configuration.BindTo<T>(typeof(T).Name);
  }
}
