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
using MangaReader.Avalonia.ViewModel.Command.Manga;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.ApplicationControl;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Core.Update;
using Client = MangaReader.Core.Client;
using LibraryViewModel = MangaReader.Avalonia.ViewModel.Explorer.LibraryViewModel;

namespace MangaReader.Avalonia
{
  class App : Application
  {
    private static ExplorerViewModel explorer;

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
      Client.Init();
      Client.OtherAppRunning += ClientOnOtherAppRunning;
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
        Updater.NewVersionFound += UpdaterOnNewVersionFound;
        lifetime.Exit += LifetimeOnExit;

        var TrayIcon = new WindowsTrayIcon();
        TrayIcon.SetIcon();
        
        var navigator = new Navigator();
        var library = new MangaReader.Core.Services.LibraryViewModel();
        var selectedMangaModels = new SelectionModel();
        var mangaModelFabric = new MangaModelFabric(navigator, new MangaSaveCommand(library, navigator), new OpenFolderCommand(selectedMangaModels, library));
        var libraryViewModel = new LibraryViewModel(navigator, TrayIcon, library, mangaModelFabric, selectedMangaModels);
        var previewFoundMangaCommandFabric = new PreviewFoundMangaCommandFabric(navigator, mangaModelFabric);
        var searchViewModel = new SearchViewModel(new MangaSearchViewModelFabric(previewFoundMangaCommandFabric));
        var mangaSettingsViewModelFabric = new MangaSettingsViewModelFabric(navigator);
        var settingsViewModel = new SettingsViewModel(navigator, mangaSettingsViewModelFabric);
        var changelogViewModel = new ChangelogViewModel();
        explorer = new ExplorerViewModel(navigator, libraryViewModel, searchViewModel, settingsViewModel, changelogViewModel, TrayIcon);

        TrayIcon.DoubleClickCommand = new ShowMainWindowCommand(explorer);
        TrayIcon.BalloonClickedCommand = new OpenFolderCommandBase();
        
        // Подключаемся к базе в отдельном потоке, чтобы не зависал UI.
        Task.Run(() => Client.Start(explorer.LoadingProcess));

        var args = Environment.GetCommandLineArgs();
        if (args.Contains("-m") || args.Contains("/min") || ConfigStorage.Instance.AppConfig.StartMinimizedToTray)
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
