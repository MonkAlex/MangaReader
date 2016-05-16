using System;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.Config
{
  public class From30To31 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var settings = Serializer<object[]>.Load(SettingsOldPath);
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

        Backup.MoveToBackup(SettingsOldPath);
      }
      catch (IndexOutOfRangeException) { }

    }

    public From30To31()
    {
      this.Version = new Version(1, 31, 5765);
    }
  }
}