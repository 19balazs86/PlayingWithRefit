using Serilog;
using Serilog.Events;

namespace PlayingWithRefit
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
      return Host
        .CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>())
        .UseSerilog(configureLogger);
    }

    private static void configureLogger(HostBuilderContext context, LoggerConfiguration configuration)
    {
      configuration
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting", LogEventLevel.Information)
        .MinimumLevel.Override("System", LogEventLevel.Information) // Gives you useful information, but not necessary.
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message}{NewLine}{Exception}");
    }
  }
}
