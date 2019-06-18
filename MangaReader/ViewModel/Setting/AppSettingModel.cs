using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Account;
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

    private int proxySettingId;

    public IEnumerable<ProxySettingModel> ProxySettingModels
    {
      get => proxySettingModels;
      set
      {
        proxySettingModels = value;
        OnPropertyChanged();
      }
    }

    private IEnumerable<ProxySettingModel> proxySettingModels;

    public ProxySettingModel SelectedProxySettingModel
    {
      get => selectedProxySettingModel;
      set
      {
        selectedProxySettingModel = value;
        if (selectedProxySettingModel != null)
          proxySettingId = selectedProxySettingModel.Id;
        OnPropertyChanged();
      }
    }

    private ProxySettingModel selectedProxySettingModel;

    public override void Load()
    {
      base.Load();

      using (var context = Repository.GetEntityContext())
      {
        this.ProxySettingModels = context.Get<ProxySetting>().Where(s => s.SettingType != ProxySettingType.Parent).Select(s => new ProxySettingModel(s)).ToList();
        this.SelectedProxySettingModel = this.ProxySettingModels.FirstOrDefault(m => m.Id == proxySettingId);
      }
    }

    public override async Task Save()
    {
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
        var config = await context.Get<DatabaseConfig>().SingleAsync().ConfigureAwait(true);
        config.FolderNamingStrategy = FolderNamingStrategy.Selected.Id;
        if (proxySettingId != config.ProxySetting.Id)
          config.ProxySetting = await context.Get<ProxySetting>().SingleAsync(s => s.Id == proxySettingId).ConfigureAwait(false);
        await context.Save(config).ConfigureAwait(true);
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
#warning DB connection in ctor
        var config = context.Get<DatabaseConfig>().Single();
        this.FolderNamingStrategy.SelectedGuid = config.FolderNamingStrategy;
        this.proxySettingId = config.ProxySetting.Id;
      }

      this.Sort = new SortModel();
      var appSort = ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription;
      if (appSort.PropertyName != null)
        Sort.SelectedDescription = appSort;
    }
  }
}
