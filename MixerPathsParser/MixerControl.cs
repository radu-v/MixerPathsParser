namespace MixerPathsParser
{
   public struct MixerControl
   {
      public string Name;
      public string Id;
      public readonly string Value;

      public MixerControl(string name, string id, string value)
      {
         Name = name;
         Id = id;
         Value = value;
      }
   }
}