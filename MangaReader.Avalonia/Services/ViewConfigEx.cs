using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using MangaReader.Core.Services.Config;
using ConfigWindowState = MangaReader.Core.Services.Config.WindowState;
using AvaloniaWindowState = global::Avalonia.Controls.WindowState;

namespace MangaReader.Avalonia.Services
{
  public static class ViewConfigEx
  {
    public static void UpdateWindowState(this ViewConfig config, Window main)
    {
      if (config.WindowStates == null)
        return;

      var monitor = GetScreens(main.Screens);
      if (config.WindowStates.Top < monitor.Height && config.WindowStates.Left < monitor.Width)
      {
        main.WindowState = (AvaloniaWindowState)Enum.Parse(typeof(ConfigWindowState), config.WindowStates.WindowState.ToString(), true);
        if (config.WindowStates.CanShow)
        {
          main.Position = new PixelPoint((int)config.WindowStates.Left, (int)config.WindowStates.Top);
          main.Width = config.WindowStates.Width;
          main.Height = config.WindowStates.Height;
        }
      }

      // TODO #115 Partialy realized.
      //void OnDisplaySettingsChanged(object sender, EventArgs args) => UpdateWindowState(config, main);
      //Microsoft.Win32.SafeHandles.DisplaySettingsChanged -= OnDisplaySettingsChanged;
      //Microsoft.Win32.SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
    }

    private static Rect GetScreens(Screens all)
    {
      var screens = all.All.OrderBy(i => i.Primary).ToList();
      var width = screens.Single(s => s.Primary).Bounds.Width;
      var height = screens.Single(s => s.Primary).Bounds.Height;
      foreach (var screen in screens.Where(s => !s.Primary))
      {
        if (screen.Bounds.TopLeft.X == width)
          width += screen.Bounds.Width;
        if (screen.Bounds.TopLeft.Y == height)
          height += screen.Bounds.Height;
      }
      return new Rect(0, 0, width, height);
    }

    public static void SaveWindowState(this ViewConfig config, Window main)
    {
      if (config.WindowStates == null)
        config.WindowStates = new WindowStates();

      var state = (ConfigWindowState)Enum.Parse(typeof(AvaloniaWindowState), main.WindowState.ToString(), true);
      if (state != ConfigWindowState.Minimized)
      {
        config.WindowStates.WindowState = state;
        config.WindowStates.Top = main.Position.Y;
        config.WindowStates.Left = main.Position.X;
        config.WindowStates.Width = main.Bounds.Width;
        config.WindowStates.Height = main.Bounds.Height;
      }
    }

  }
}