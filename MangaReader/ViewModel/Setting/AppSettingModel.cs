using System.Collections.Generic;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.UI.Skin;

namespace MangaReader.ViewModel.Setting
{
  public class AppSettingModel : SettingViewModel
  {
    private bool updateReader;
    private bool minimizeToTray;
    private Languages language;
    private string autoUpdateHours;
    private ISkinSetting skin;

    public bool UpdateReader
    {
      get { return updateReader; }
      set
      {
        updateReader = value;
        OnPropertyChanged();
      }
    }

    public bool MinimizeToTray
    {
      get { return minimizeToTray; }
      set
      {
        minimizeToTray = value;
        OnPropertyChanged();
      }
    }

    public Languages Language
    {
      get { return language; }
      set
      {
        language = value;
        OnPropertyChanged();
      }
    }

    public List<Languages> Languages { get; private set; }

    public string AutoUpdateHours
    {
      get { return autoUpdateHours; }
      set
      {
        autoUpdateHours = value;
        OnPropertyChanged();
      }
    }

    public ISkinSetting Skin
    {
      get { return skin; }
      set
      {
        skin = value;
        OnPropertyChanged();
      }
    }

    public IReadOnlyList<ISkinSetting> SkinSettings { get; private set; } 

    public override void Save()
    {
      base.Save();

      var appConfig = ConfigStorage.Instance.AppConfig;
      appConfig.Language = Language;
      appConfig.UpdateReader = UpdateReader;
      appConfig.MinimizeToTray = MinimizeToTray;

      int hour;
      if (int.TryParse(AutoUpdateHours, out hour))
        appConfig.AutoUpdateInHours = hour;

      ConfigStorage.Instance.ViewConfig.SkinGuid = Skin.Guid;
    }

    public AppSettingModel()
    {
      this.Header = "Основные настройки";
      this.Languages = Generic.GetEnumValues<Languages>();
      this.SkinSettings = Skins.SkinSettings;

      var appConfig = ConfigStorage.Instance.AppConfig;
      this.UpdateReader = appConfig.UpdateReader;
      this.MinimizeToTray = appConfig.MinimizeToTray;
      this.Language = appConfig.Language;
      this.AutoUpdateHours = appConfig.AutoUpdateInHours.ToString();
      this.Skin = Skins.GetSkinSetting(ConfigStorage.Instance.ViewConfig.SkinGuid);
    }
  }
}