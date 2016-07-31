using System;
using System.IO;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.Config
{
  public class FromBaseTo30 : ConfigConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && File.Exists(SettingsOldPath);
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var settings = Serializer<object[]>.Load(SettingsOldPath);
      if (settings == null)
        return;

      try
      {
        if (settings[1] is bool)
          ConfigStorage.Instance.DatabaseConfig.MangaSettings.ForEach(ms => ms.OnlyUpdate = (bool)settings[1]);
        if (settings[4] is bool)
          ConfigStorage.Instance.DatabaseConfig.MangaSettings.ForEach(ms => ms.CompressManga = (bool)settings[4]);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex);
      }
    }

    public FromBaseTo30()
    {
      this.Version = new Version(1, 30, 5765);
    }
  }
}