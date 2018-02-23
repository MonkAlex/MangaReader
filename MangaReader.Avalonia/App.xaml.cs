using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Logging.Serilog;
using Avalonia.Themes.Default;
using Avalonia.Markup.Xaml;
using Serilog;

namespace MangaReader.Avalonia
{
  class App : Application
  {
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
      .UsePlatformDetect()
      .UseReactiveUI();

    public override void Initialize()
    {
      AvaloniaXamlLoader.Load(this);
      base.Initialize();
    }

    static void Main(string[] args)
    {
      BuildAvaloniaApp().Start<MainWindow>();
    }

    public static void AttachDevTools(Window window)
    {
#if DEBUG
      DevTools.Attach(window);
#endif
    }
  }
}
