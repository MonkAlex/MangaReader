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
    public static AppBuilder BuildAvaloniaApp()
    {
      var app = AppBuilder.Configure<App>();

      if (Environment.OSVersion.Platform == PlatformID.Unix ||
          Environment.OSVersion.Platform == PlatformID.MacOSX)
        app = app.UseSkia().UseGtk3();
      else
        app = app.UsePlatformDetect();
      app = app.UseReactiveUI();
      return app;
    }

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
