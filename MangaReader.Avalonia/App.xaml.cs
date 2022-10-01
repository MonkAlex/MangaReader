using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using MangaReader.Avalonia.Platform.Win;
using MangaReader.Avalonia.Services;
using MangaReader.Avalonia.ViewModel;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Avalonia.ViewModel.Command.Library;
using MangaReader.Avalonia.ViewModel.Command.Manga;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core;
using MangaReader.Core.ApplicationControl;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Core.Update;

namespace MangaReader.Avalonia
{
  class App : Application
  {
    private static ExplorerViewModel explorer;
    private static ClientInit clientInit;

    public static AppBuilder BuildAvaloniaApp()
      => AppBuilder
        .Configure<App>()
        .UsePlatformDetect()
        .With(new X11PlatformOptions() { UseEGL = true })
        .With(new Win32PlatformOptions() { AllowEglInitialization = true, UseDeferredRendering = false })
        .UseReactiveUI();

    public override void Initialize()
    {
      AvaloniaXamlLoader.Load(this);
      base.Initialize();
    }

    private static void LifetimeOnExit(object sender, ControlledApplicationLifetimeExitEventArgs e)
    {
      new ExitCommand(explorer).Execute(sender);
    }

    static void Main(string[] args)
    {
      clientInit = new ClientInit();
      clientInit.Init();
      clientInit.OtherAppRunning += ClientOnOtherAppRunning;
      BuildAvaloniaApp().StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);
    }

    private static void ClientOnOtherAppRunning(object sender, string e)
    {
      if (!Messages.TryParse(e, true, out Messages message))
        return;

      switch (message)
      {
        case Messages.Activate:
          Dispatcher.UIThread.InvokeAsync(() =>
          {
            new ShowMainWindowCommand(explorer).Execute(null);
          });
          break;
        case Messages.AddManga:
          Log.Add($"Accept message to add new manga, but not implemented now.");
          break;
        case Messages.Close:
          Dispatcher.UIThread.InvokeAsync(() =>
          {
            new ExitCommand(explorer).Execute(null);
          });
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private static async void UpdaterOnNewVersionFound(object sender, string e)
    {
      var dialog = new Dialogs.Avalonia.Dialog
      {
        Title = $"Найдено обновление {e}",
        Description = "Автоматическое обновление не реализовано в текущей версии, обновитесь вручную."
      };
      var download = dialog.Buttons.AddButton("Скачать обновление");
      var close = dialog.Buttons.AddButton("Закрыть");

      await Dispatcher.UIThread.InvokeAsync(async () =>
      {
        if (await dialog.ShowAsync().ConfigureAwait(true) == download)
        {
          Helper.StartUseShell(Updater.RepositoryReleaseUri);
        }
      }).LogException().ConfigureAwait(true);
    }

    public override void OnFrameworkInitializationCompleted()
    {
      if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
      {
        lifetime.Exit += LifetimeOnExit;

        var TrayIcon = new WindowsTrayIcon();
        TrayIcon.SetIcon();

        var environments = new Environments();
        var configStorage = new JsonConfigStorage(environments.ConfigPath);
        var config = configStorage.Load();
        var updater = new Updater(environments, config);
        updater.NewVersionFound += UpdaterOnNewVersionFound;
        var loader = new Loader(environments);
        var pluginManager = new PluginManager(environments.PluginPath, loader);
        var mapping = new MangaReader.Core.NHibernate.Mapping(environments);
        
        var navigator = new Navigator();
        var library = new MangaReader.Core.Services.LibraryViewModel(config);
        var selectedMangaModels = new SelectionModel();
        var openFolder = new OpenFolderCommand(selectedMangaModels, library, new OpenFolderCommandBase());
        var mangaModelFabric = new MangaModelFabric(navigator, new MangaSaveCommand(library, navigator), openFolder);
        var updateCommand = new UpdateVisibleMangaCommand(library);
        var mangaCommands = new BaseCommand[] {
          openFolder,
          new ChangeUpdateMangaCommand(false, selectedMangaModels, library),
          new ChangeUpdateMangaCommand(true, selectedMangaModels, library),
          new UpdateMangaCommand(selectedMangaModels, library),
          new CompressMangaCommand(selectedMangaModels, library),
          new OpenUrlMangaCommand(selectedMangaModels, library),
          new HistoryClearMangaCommand(selectedMangaModels, library),
          new DeleteMangaCommand(selectedMangaModels, library),
          new ShowPropertiesMangaCommand(selectedMangaModels, library, navigator, mangaModelFabric)
        };
        var libraryCommands = new[] { new UpdateWithPauseCommand(updateCommand, new PauseCommand(library), new ContinueCommand(library), library) };
        var libraryViewModel = new ViewModel.Explorer.LibraryViewModel(navigator, TrayIcon, library, mangaModelFabric, selectedMangaModels, libraryCommands, mangaCommands);
        var searchViewModel = new SearchViewModel(new MangaSearchViewModelFabric(navigator, mangaModelFabric));
        var proxySettingsFabric = new ProxySettingModelFabric();
        var mangaSettingsViewModelFabric = new MangaSettingsViewModelFabric(navigator, proxySettingsFabric);
        var proxySettingSelector = new ProxySettingSelectorModel(navigator, proxySettingsFabric);
        var settingsViewModel = new SettingsViewModel(navigator, mangaSettingsViewModelFabric, proxySettingsFabric, proxySettingSelector);
        var changelogViewModel = new ChangelogViewModel();
        var process = new ProcessModel();
        var tabs = new ExplorerTabViewModel[] { libraryViewModel, searchViewModel, settingsViewModel };
        var bottomTabs = new ExplorerTabViewModel[] { changelogViewModel };
        explorer = new ExplorerViewModel(navigator, tabs, bottomTabs, TrayIcon, process);

        // Post configuration dependency
        updateCommand.SetViewModel(libraryViewModel);
        TrayIcon.DoubleClickCommand = new ShowMainWindowCommand(explorer);
        TrayIcon.BalloonClickedCommand = new OpenFolderCommandBase();

        // DB connection in another thread, then UI work well
        var client = new Core.Client(configStorage, pluginManager, mapping, clientInit, updater);
        Task.Run(() => client.Start(process));

        var args = Environment.GetCommandLineArgs();
        if (args.Contains("-m") || args.Contains("/min") || config.AppConfig.StartMinimizedToTray)
        {
          // SaveSettingsCommand.ValidateMangaPaths();
        }
        else
        {
          ShowMainWindowCommand.SetLifetime(explorer, lifetime);
        }

      }

      base.OnFrameworkInitializationCompleted();
    }
  }
}
