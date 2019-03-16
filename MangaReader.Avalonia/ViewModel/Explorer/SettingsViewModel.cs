using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class SettingsViewModel : ExplorerTabViewModel
  {
    public int AutoupdateLibraryInHours
    {
      get => autoupdateLibraryInHours;
      set => RaiseAndSetIfChanged(ref autoupdateLibraryInHours, value);
    }

    private int autoupdateLibraryInHours;

    public bool CheckAppUpdateOnStart
    {
      get => checkAppUpdateOnStart;
      set => RaiseAndSetIfChanged(ref checkAppUpdateOnStart, value);
    }

    private bool checkAppUpdateOnStart;

    public bool MinimizeToTray
    {
      get => minimizeToTray;
      set => RaiseAndSetIfChanged(ref minimizeToTray, value);
    }

    private bool minimizeToTray;

    public Languages Language
    {
      get => language;
      set => RaiseAndSetIfChanged(ref language, value);
    }

    private Languages language;

    public List<Languages> AllowedLanguages => Generic.GetEnumValues<Languages>();

    public IFolderNamingStrategy FolderNamingStrategy
    {
      get => folderNamingStrategy;
      set => RaiseAndSetIfChanged(ref folderNamingStrategy, value);
    }

    private IFolderNamingStrategy folderNamingStrategy;

    public List<IFolderNamingStrategy> FolderNamingStrategies => Core.Services.FolderNamingStrategies.Strategies.ToList();

    public IReadOnlyList<SortSetting> SortSettings => SortSetting.Sorts;

    public SortSetting SortSetting
    {
      get => sortSetting;
      set => RaiseAndSetIfChanged(ref sortSetting, value);
    }

    private SortSetting sortSetting;

    public override async Task OnSelected(ExplorerTabViewModel previousModel)
    {
      await base.OnSelected(previousModel).ConfigureAwait(true);

      if (!ExplorerViewModel.Instance.Tabs.OfType<MangaSettingsViewModel>().Any())
      {
        using (var context = Repository.GetEntityContext("Load manga settings"))
        {
          ReloadConfig();
          var settings = context.Get<MangaSetting>().ToList();
          ExplorerViewModel.Instance.Tabs.AddRange(settings.Select(s => new MangaSettingsViewModel(s)));
        }
      }
    }

    public override async Task OnUnselected(ExplorerTabViewModel newModel)
    {
      await base.OnUnselected(newModel).ConfigureAwait(true);
      if (!(newModel is SettingsViewModel || newModel is MangaSettingsViewModel))
      {
        foreach (var tab in ExplorerViewModel.Instance.Tabs.OfType<MangaSettingsViewModel>().ToList())
          ExplorerViewModel.Instance.Tabs.Remove(tab);

#warning Нужно ресетить только после изменения порядка сортировки.
        foreach (var model in ExplorerViewModel.Instance.Tabs.OfType<LibraryViewModel>())
          model.FilteredItems.Reset();
      }
    }

    public ICommand Save { get; }

    public ICommand UndoChanged { get; }

    private void ReloadConfig()
    {
      var appConfig = ConfigStorage.Instance.AppConfig;
      this.CheckAppUpdateOnStart = appConfig.UpdateReader;
      this.MinimizeToTray = appConfig.MinimizeToTray;
      this.AutoupdateLibraryInHours = appConfig.AutoUpdateInHours;
      this.Language = appConfig.Language;

      var viewConfig = ConfigStorage.Instance.ViewConfig;
      this.SortSetting = SortSettings
        .OrderByDescending(s => s.SortDescription.PropertyName == viewConfig.LibraryFilter.SortDescription.PropertyName)
        .FirstOrDefault();

      using (var context = Repository.GetEntityContext())
      {
        var config = context.Get<DatabaseConfig>().Single();
        this.FolderNamingStrategy = FolderNamingStrategies.FirstOrDefault(s => s.Id == config.FolderNamingStrategy);
      }
    }

    private async Task SaveConfig()
    {
      var configStorage = ConfigStorage.Instance;
      var appConfig = configStorage.AppConfig;
      appConfig.UpdateReader = this.CheckAppUpdateOnStart;
      appConfig.MinimizeToTray = this.MinimizeToTray;
      appConfig.AutoUpdateInHours = this.AutoupdateLibraryInHours;
      appConfig.Language = this.Language;

      var viewConfig = configStorage.ViewConfig;
      viewConfig.LibraryFilter.SortDescription = SortSetting.SortDescription;

      configStorage.Save();

      using (var context = Repository.GetEntityContext())
      {
        var config = context.Get<DatabaseConfig>().Single();
        config.FolderNamingStrategy = FolderNamingStrategy.Id;
        await context.Save(config).ConfigureAwait(true);
      }
    }

    public SettingsViewModel()
    {
      this.Name = "Settings";
      this.Priority = 100;

      this.Save = new DelegateCommand(SaveConfig, () => true);
      this.UndoChanged = new DelegateCommand(ReloadConfig);
    }
  }
}