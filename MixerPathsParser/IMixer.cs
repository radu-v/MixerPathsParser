using System.Collections.Generic;

namespace MixerPathsParser
{
   public interface IMixer
   {
      IDictionary<string, MixerControl> MixerControls { get; }
      IDictionary<string, MixerPath> MixerPaths { get; }
      void Load(string mixerPathsXml);
   }
}