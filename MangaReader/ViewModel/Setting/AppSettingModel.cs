using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.NHibernate;
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
    private SkinSetting skin;
    private bool startAppMinimizedToTray;

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

    public SkinSetting Skin
    {
      get { return skin; }
      set
      {
        skin = value;
        OnPropertyChanged();
      }
    }

    public bool StartAppMinimizedToTray
    {
      get { return startAppMinimizedToTray; }
      set
      {
        startAppMinimizedToTray = value;
        OnPropertyChanged();
      }
    }

    public IReadOnlyList<SkinSetting> SkinSettings { get; private set; }

    public FolderNamingModel FolderNamingStrategy { get; set; }

    public SortModel Sort { get; set; }

    public override void Save()
    {
      base.Save();

      var appConfig = ConfigStorage.Instance.AppConfig;
      appConfig.Language = Language;
      appConfig.UpdateReader = UpdateReader;
      appConfig.MinimizeToTray = MinimizeToTray;
      appConfig.StartMinimizedToTray = StartAppMinimizedToTray;

      int hour;
      if (int.TryParse(AutoUpdateHours, out hour))
        appConfig.AutoUpdateInHours = hour;

      var viewConfig = ConfigStorage.Instance.ViewConfig;
      if (Skin.Guid != Default.DefaultGuid || viewConfig.SkinGuid != Guid.Empty)
        viewConfig.SkinGuid = Skin.Guid;

      viewConfig.LibraryFilter.SortDescription = Sort.SelectedDescription;

      using (var context = Repository.GetEntityContext())
      {
        var config = context.Get<DatabaseConfig>().Single();
        config.FolderNamingStrategy = FolderNamingStrategy.Selected.Id;
        config.Save();
      }
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
      this.StartAppMinimizedToTray = appConfig.StartMinimizedToTray;

      this.FolderNamingStrategy = new FolderNamingModel();
      using (var context = Repository.GetEntityContext())
      {
        var config = context.Get<DatabaseConfig>().Single();
        this.FolderNamingStrategy.SelectedGuid = config.FolderNamingStrategy;
      }

      this.Sort = new SortModel();
      var appSort = ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription;
      if (appSort.PropertyName != null)
        Sort.SelectedDescription = appSort;
    }
  }
}