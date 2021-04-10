using Microsoft.Extensions.DependencyInjection;

namespace MixerPathsParser
{
   class Program
   {
      static void Main(string[] args)
      {
         var services = Startup.ConfigureServices();
         var serviceProvider = services.BuildServiceProvider();

         serviceProvider.GetService<EntryPoint>().Run(args);
      }
   }
}