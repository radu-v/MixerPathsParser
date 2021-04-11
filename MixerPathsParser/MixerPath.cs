using System.Collections.Generic;

namespace MixerPathsParser
{
   public record MixerPath(string Name)
   {
      public IList<MixerControl> MixerControls = new List<MixerControl>();
      public IList<MixerPath> MixerPaths = new List<MixerPath>();
   }
}