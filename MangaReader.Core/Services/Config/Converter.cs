using System;
using System.Linq;
using MangaReader.Manga.Acomic;
using MangaReader.Manga.Grouple;

namespace MangaReader.Services.Config
{
  internal static class Converter
  {
    internal static void ConvertAll(IProcess process)
    {
      // 1.* To 1.30
      ConvertBaseTo30(process);

      // to 1.31.5765
      Convert30To31(process);
    }

#pragma warning disable CS0618 // Type or member is obsolete
    private static void Convert30To31(IProcess process)
    {
      var version = new Version(1, 31, 5765);
      if (version.CompareTo(ConfigStorage.Instance.DatabaseConfig.Version) > 0 && process.Version.CompareTo(version) >= 0)
      {
        var settings = Serializer<object[]>.Load(ConfigStorage.SettingsOldPath);
        if (settings == null)
          return;

        try
        {
          ConfigStorage.Instance.AppConfig.Language = (Languages)settings[0];
          ConfigStorage.Instance.AppConfig.UpdateReader = (bool)settings[2];
          ConfigStorage.Instance.AppConfig.MinimizeToTray = (bool)settings[7];
          ConfigStorage.Instance.AppConfig.AutoUpdateInHours = (int)settings[8];

          ConfigStorage.Instance.ViewConfig.WindowStates.Top = (double)((object[])settings[5])[0];
          ConfigStorage.Instance.ViewConfig.WindowStates.Left = (double)((object[])settings[5])[1];
          ConfigStorage.Instance.ViewConfig.WindowStates.Width = (double)((object[])settings[5])[2];
          ConfigStorage.Instance.ViewConfig.WindowStates.Height = (double)((object[])settings[5])[3];
          ConfigStorage.Instance.ViewConfig.WindowStates.WindowState = (WindowState)((object[])settings[5])[4];

          ConfigStorage.Instance.DatabaseConfig.Version = new Version((string)settings[9]);

          Backup.MoveToBackup(ConfigStorage.SettingsOldPath);
        }
        catch (IndexOutOfRangeException) { }
      }
    }

    private static void ConvertBaseTo30(IProcess process)
    {
      var version = new Version(1, 30, 5765);
      if (version.CompareTo(ConfigStorage.Instance.DatabaseConfig.Version) > 0 && process.Version.CompareTo(version) >= 0)
      {
        var settings = Serializer<object[]>.Load(ConfigStorage.SettingsOldPath);
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
        catch (Exception ex)
        {
          Log.Exception(ex);
        }
      }
    }
#pragma warning restore CS0618 // Type or member is obsolete
  }
}