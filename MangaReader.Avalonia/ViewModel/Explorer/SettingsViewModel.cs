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

    public override async Task OnSelected(ExplorerTabViewModel previousModel)
    {
      await base.OnSelected(previousModel);

      if (!ExplorerViewModel.Instance.Tabs.OfType<MangaSettingsViewModel>().Any())
      {
        using (var context = Repository.GetEntityContext())
        {
          var settings = context.Get<MangaSetting>().ToList();
          ExplorerViewModel.Instance.Tabs.AddRange(settings.Select(s => new MangaSettingsViewModel(s)));
        }
      }
    }

    public override async Task OnUnselected(ExplorerTabViewModel newModel)
    {
      await base.OnUnselected(newModel);
      if (!(newModel is SettingsViewModel || newModel is MangaSettingsViewModel))
      {
        foreach (var tab in ExplorerViewModel.Instance.Tabs.OfType<MangaSettingsViewModel>().ToList())
          ExplorerViewModel.Instance.Tabs.Remove(tab);
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
    }

    private void SaveConfig()
    {
      var configStorage = ConfigStorage.Instance;
      var appConfig = configStorage.AppConfig;
      appConfig.UpdateReader = this.CheckAppUpdateOnStart;
      appConfig.MinimizeToTray = this.MinimizeToTray;
      appConfig.AutoUpdateInHours = this.AutoupdateLibraryInHours;
      configStorage.Save();
    }

    public SettingsViewModel()
    {
      this.Name = "Settings";
      this.Priority = 100;

      ReloadConfig();
      this.Save = new DelegateCommand(SaveConfig);
      this.UndoChanged = new DelegateCommand(ReloadConfig);
    }
  }
}