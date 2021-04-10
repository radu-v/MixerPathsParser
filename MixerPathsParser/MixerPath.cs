using System.Collections.Generic;

namespace MixerPathsParser
{
   public struct MixerPath
   {
      public readonly string Name;

      public IList<MixerControl> MixerControls;
      public IList<MixerPath> MixerPaths;

      public MixerPath(string name)
      {
         Name = name;

         MixerControls = new List<MixerControl>();
         MixerPaths = new List<MixerPath>();
      }
   }
}