using Microsoft.Extensions.Logging;

namespace MixerPathsParser
{
   public class EntryPoint
   {
      readonly ILogger<EntryPoint> _logger;
      readonly IMixer _mixer;

      public EntryPoint(ILogger<EntryPoint> logger, IMixer mixer)
      {
         _logger = logger;
         _mixer = mixer;
      }
      public void Run(string[] args)
      {
         _logger.LogDebug("Starting");
         _mixer.Load(args[0]);
         _logger.LogDebug("Done");
      }
   }
}