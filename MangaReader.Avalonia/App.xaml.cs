using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Markup.Xaml;
using MangaReader.Avalonia.ViewModel.Command;

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

    protected override void OnExiting(object sender, EventArgs e)
    {
      base.OnExiting(sender, e);
      new ExitCommand().Execute(sender);
    }

    static void Main(string[] args)
    {
      MangaReader.Core.Client.Init();
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
