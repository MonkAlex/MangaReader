using System;
using System.Linq;
using System.Windows;
using MangaReader.Core.Services.Config;
using MangaReader.UI;
using MangaReader.UI.MainForm;
using WindowState = MangaReader.Core.Services.Config.WindowState;

namespace MangaReader.Services.Config
{
  public static class ViewConfigWPF
  {
    public static void UpdateWindowState(this ViewConfig config, Window main)
    {
      if (config.WindowStates == null)
        return;

      var monitor = Monitors.FullScreen();
      if (config.WindowStates.Top < monitor.Height && config.WindowStates.Left < monitor.Width)
      {
        main.WindowState = (System.Windows.WindowState)Enum.Parse(typeof(WindowState), config.WindowStates.WindowState.ToString(), true);
        if (config.WindowStates.CanShow)
        {
          main.Top = config.WindowStates.Top;
          main.Left = config.WindowStates.Left;
          main.Width = config.WindowStates.Width;
          main.Height = config.WindowStates.Height;
        }
      }
    }

    public static void SaveWindowState(this ViewConfig config, Window main)
    {
      if (config.WindowStates == null)
        config.WindowStates = new WindowStates();
      
      var state = (WindowState) Enum.Parse(typeof (System.Windows.WindowState), main.WindowState.ToString(), true);
      if (state != WindowState.Minimized)
      {
        config.WindowStates.WindowState = state;
        config.WindowStates.Top = main.Top;
        config.WindowStates.Left = main.Left;
        config.WindowStates.Width = main.Width;
        config.WindowStates.Height = main.Height;
      }
    }

  }
}