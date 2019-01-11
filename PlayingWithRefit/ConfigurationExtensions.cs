using Microsoft.Extensions.Configuration;

namespace PlayingWithRefit
{
  public static class ConfigurationExtensions
  {
    public static T BindTo<T>(this IConfiguration configuration) where T : new()
    {
      T bindingObject = new T();

      string sectionName = bindingObject.GetType().Name;

      configuration.GetSection(sectionName).Bind(bindingObject);

      return bindingObject;
    }
  }
}
