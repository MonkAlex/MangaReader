using System.Collections.Generic;
using MangaReader.Core.Services.Config;
using MangaReader.Services;

namespace MangaReader.ViewModel.Setting
{
  public class AppSettingModel : SettingViewModel
  {
    private bool updateReader;
    private bool minimizeToTray;
    private Languages language;
    private string autoUpdateHours;

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

    public override void Save()
    {
      base.Save();

      ConfigStorage.Instance.AppConfig.Language = Language;
      ConfigStorage.Instance.AppConfig.UpdateReader = UpdateReader;
      ConfigStorage.Instance.AppConfig.MinimizeToTray = MinimizeToTray;

      int hour;
      if (int.TryParse(AutoUpdateHours, out hour))
        ConfigStorage.Instance.AppConfig.AutoUpdateInHours = hour;
    }

    public AppSettingModel()
    {
      this.Header = "Основные настройки";
      this.Languages = Generic.GetEnumValues<Languages>();

      this.UpdateReader = ConfigStorage.Instance.AppConfig.UpdateReader;
      this.MinimizeToTray = ConfigStorage.Instance.AppConfig.MinimizeToTray;
      this.Language = ConfigStorage.Instance.AppConfig.Language;
      this.AutoUpdateHours = ConfigStorage.Instance.AppConfig.AutoUpdateInHours.ToString();
    }
  }
}