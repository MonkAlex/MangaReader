using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Diagnostics;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using MangaReader.Avalonia.ViewModel;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Core;
using MangaReader.Core.Services;
using MangaReader.Core.Update;

namespace MangaReader.Avalonia
{
  class App : Application
  {
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UsePlatformDetect().UseReactiveUI();

    public override void Initialize()
    {
      AvaloniaXamlLoader.Load(this);
      base.Initialize();
    }

    private static void LifetimeOnExit(object sender, ControlledApplicationLifetimeExitEventArgs e)
    {
      new ExitCommand().Execute(sender);
    }

    static void Main(string[] args)
    {
      Client.Init();
      BuildAvaloniaApp().StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);
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

        var explorer = ExplorerViewModel.Instance;

        // Подключаемся к базе в отдельном потоке, чтобы не зависал UI.
        Task.Run(() => Client.Start(explorer.LoadingProcess));

        var window = new MainWindow();
        explorer.LoadingProcess.Status = window.Title;
        window.DataContext = explorer;
        lifetime.MainWindow = window;
      }

      base.OnFrameworkInitializationCompleted();
    }

    public static void AttachDevTools(Window window)
    {
#if DEBUG
      DevTools.Attach(window);
#endif
    }
  }
}
