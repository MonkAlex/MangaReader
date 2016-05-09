using System;
using System.Collections.Generic;
using System.Linq;

namespace MangaReader.UI.Skin
{
  public class Skins
  {
    public static IEnumerable<ISkinSetting> GetSkinSettings()
    {
      var types = Core.ResolveAssembly.AllowedAssemblies()
          .SelectMany(a => a.GetTypes())
          .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(ISkinSetting)));
      foreach (var type in types)
      {
        yield return (ISkinSetting)Activator.CreateInstance(type);
      }
    }

    public static ISkinSetting GetSkinSetting(Guid guid)
    {
      ISkinSetting defaultSkin = null;
      foreach (var setting in GetSkinSettings())
      {
        if (setting.Guid == Default.DefaultGuid)
          defaultSkin = setting;
        if (guid == setting.Guid)
          return setting;
      }
      return defaultSkin;
    }
  }
}