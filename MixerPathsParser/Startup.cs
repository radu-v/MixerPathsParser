using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MixerPathsParser
{
   public static class Startup
   {
      public static IServiceCollection ConfigureServices()
      {
         var services = new ServiceCollection();
         var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, false);

         var configuration = builder.Build();

         Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
         
         return services
            .AddSingleton(configuration)
            .AddLogging(b => b
               .AddConfiguration(configuration.GetSection("Logging"))
               .AddSerilog())
            .AddTransient<EntryPoint>()
            .AddTransient<IMixer, Mixer>();
      }
   }
}