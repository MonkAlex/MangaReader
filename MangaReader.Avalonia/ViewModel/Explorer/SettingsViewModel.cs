using System.Windows.Input;
using MangaReader.Avalonia.ViewModel.Command;
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