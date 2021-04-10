using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace MixerPathsParser
{
   public class Mixer : IMixer
   {
      readonly ILogger<Mixer> _logger;
      public IDictionary<string, MixerControl> MixerControls { get; }
      public IDictionary<string, MixerPath> MixerPaths { get; }

      public Mixer(ILogger<Mixer> logger)
      {
         _logger = logger;
         MixerControls = new Dictionary<string, MixerControl>();
         MixerPaths = new Dictionary<string, MixerPath>();
      }

      public void Load(string mixerPathsXml)
      {
         var xdoc = XDocument.Load(mixerPathsXml);

         if (xdoc.Root?.Name != "mixer")
            throw new Exception("Invalid mixer_paths xml passed.");

         var root = xdoc.Root;

         PopulateMixerControls(root.Elements("ctl"));
         PopulateMixerPaths(root.Elements("path"));
      }

      void PopulateMixerControls(IEnumerable<XElement> ctlElements)
      {
         var xElements = ctlElements as XElement[] ?? ctlElements.ToArray();

         for (var i = 0; i < xElements.Length; i++)
         {
            var ctlElement = xElements[i];
            var name = ctlElement.Attribute("name")?.Value;

            if (string.IsNullOrWhiteSpace(name))
            {
               _logger.LogWarning("ctl with no name at index {CtlIndex}, skipping", i);
               continue;
            }

            var id = ctlElement.Attribute("id")?.Value;
            var ctlKey = $"{name}:{id ?? ""}";

            if (MixerControls.ContainsKey(ctlKey))
            {
               if (string.IsNullOrWhiteSpace(id))
                  _logger.LogWarning("Duplicate control {CtlName}, skipping", name);
               else
                  _logger.LogWarning("Duplicate control {CtlName} id {CtlId}, skipping", name, id);
               
               continue;
            }

            var value = ctlElement.Attribute("value")?.Value;
            MixerControls[ctlKey] = new MixerControl(name, id, value);
            _logger.LogDebug("Found control {CtlName} with default value {CtlValue}", name, value);
         }
      }

      void PopulateMixerPaths(IEnumerable<XElement> pathElements)
      {
         var xElements = pathElements as XElement[] ?? pathElements.ToArray();

         for (var i = 0; i < xElements.Length; i++)
         {
            var pathElement = xElements[i];
            var name = pathElement.Attribute("name")?.Value;
            
            if (string.IsNullOrWhiteSpace(name))
            {
               _logger.LogWarning("ctl with no name at index {CtlIndex}, skipping", i);
               continue;
            }
            
            if (MixerPaths.ContainsKey(name))
            {
               _logger.LogWarning("Duplicate path {PathName}, skipping", name);
               continue;
            }

            var path = new MixerPath(name);
            AddControlsToPath(path, pathElement.Elements("ctl"));
            AddParentPathsToPath(path, pathElement.Elements("path"));

            MixerPaths[name] = path;
         }
      }

      void AddControlsToPath(MixerPath path, IEnumerable<XElement> ctlElements)
      {
         var xElements = ctlElements as XElement[] ?? ctlElements.ToArray();

         for (var i = 0; i < xElements.Length; i++)
         {
            var ctlElement = xElements[i];
            var name = ctlElement.Attribute("name")?.Value;

            if (string.IsNullOrWhiteSpace(name))
            {
               _logger.LogWarning("ctl with no name in path {PathName} at index {CtlIndex}, skipping", path.Name, i);
               continue;
            }

            var id = ctlElement.Attribute("id")?.Value;
            var ctlKey = $"{name}:{id ?? ""}";

            if (!MixerControls.TryGetValue(ctlKey, out var ctl))
            {
               if (string.IsNullOrWhiteSpace(id))
                  _logger.LogWarning("Path {PathName} uses undefined control {CtlName}", path.Name, name);
               else
                  _logger.LogWarning("Path {PathName} uses undefined control {CtlName}, id {CtlId}", path.Name, name, id);
               continue;
            }

            var value = ctlElement.Attribute("value")?.Value;
            path.MixerControls.Add(new MixerControl(name, id, value));
            
            if (string.IsNullOrWhiteSpace(id))
               _logger.LogDebug("Path {PathName} uses control {CtlName}", path.Name, name);
            else
               _logger.LogDebug("Path {PathName} uses control {CtlName} id {CtlId}, value {CtlDefaultValue} => {CtlValue}", path.Name, name, id, ctl.Value, value);
         }
      }

      void AddParentPathsToPath(MixerPath path, IEnumerable<XElement> pathElements)
      {
         var xElements = pathElements as XElement[] ?? pathElements.ToArray();

         foreach (var pathElement in xElements)
         {
            var name = pathElement.Attribute("name")?.Value;

            if (string.IsNullOrWhiteSpace(name))
            {
               _logger.LogWarning("parent path with no name in path {PathName}, skipping", path.Name);
               continue;
            }

            if (!MixerPaths.ContainsKey(name))
            {
               _logger.LogWarning("Path {PathName} uses undefined path {ParentPathName}", path.Name, name);
               continue;
            }

            var value = pathElement.Attribute("value")?.Value;
            path.MixerPaths.Add(new MixerPath(name));
            _logger.LogDebug("Path {PathName} uses path {ParentPathName}", path.Name, name);
         }
      }
   }
}