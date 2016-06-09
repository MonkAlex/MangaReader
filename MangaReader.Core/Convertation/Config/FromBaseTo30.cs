using System;
using System.IO;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga.Acomic;
using MangaReader.Core.Manga.Grouple;
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
        if (settings[3] is object[])
        {
          var setting = settings[3] as object[];
          if (ConfigStorage.Instance.DatabaseConfig.MangaSettings.FirstOrDefault(a => a.Manga == Readmanga.Type) ==
              null)
            new MangaSetting()
            {
              Folder = setting[0] as string,
              Manga = Readmanga.Type,
              MangaName = "Readmanga",
              DefaultCompression = Compression.CompressionMode.Volume
            }.Save();
          if (ConfigStorage.Instance.DatabaseConfig.MangaSettings.FirstOrDefault(a => a.Manga == Acomics.Type) == null)
            new MangaSetting()
            {
              Folder = setting[1] as string,
              Manga = Acomics.Type,
              MangaName = "Acomics",
              DefaultCompression = Compression.CompressionMode.Manga
            }.Save();
        }
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