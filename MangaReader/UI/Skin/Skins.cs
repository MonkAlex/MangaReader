using System;
using System.Collections.Generic;
using System.Linq;

namespace MangaReader.UI.Skin
{
  public class Skins
  {
    private static IReadOnlyList<SkinSetting> skinSettings;

    public static IReadOnlyList<SkinSetting> SkinSettings
    {
      get { return skinSettings ?? (skinSettings = GetSkinSettings().ToList().AsReadOnly()); }
    }

    private static IEnumerable<SkinSetting> GetSkinSettings()
    {
      var types = Core.ResolveAssembly.AllowedAssemblies()
          .SelectMany(a => a.GetTypes())
          .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(SkinSetting)));
      foreach (var type in types)
      {
        yield return (SkinSetting)Activator.CreateInstance(type);
      }
    }

    public static SkinSetting GetSkinSetting(Guid guid)
    {
      SkinSetting defaultSkin = null;
      foreach (var setting in SkinSettings)
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