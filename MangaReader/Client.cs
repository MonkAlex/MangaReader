using System;
using System.Windows;
using System.Windows.Controls;
using MangaReader.Core.ApplicationControl;
using MangaReader.UI.Services;
using MangaReader.ViewModel;

namespace MangaReader
{
  public static class Client
  {
    public static void Run()
    {
      ViewResolver.Instance.ViewInit();
      MangaReader.Core.Client.OtherAppRunning += ClientOnOtherAppRunning;

      var model = new Initialize();
      model.Show();

      WindowModel.Instance.Show();
    }

    private static void ClientOnOtherAppRunning(object sender, string s)
    {
      Messages message;
      if (!Messages.TryParse(s, true, out message))
        return;

      switch (message)
      {
        case Messages.Activate:
          App.Current.Dispatcher.Invoke(() =>
          {
            var window = App.Current.MainWindow;

            var state = WindowState.Normal;
            if (window.WindowState != WindowState.Minimized)
            {
              state = window.WindowState;
              window.WindowState = WindowState.Minimized;
            }

            if (window.WindowState == WindowState.Minimized)
              window.WindowState = state;
          });
          break;
        case Messages.AddManga:
          break;
        case Messages.Close:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public static void Close()
    {
      MangaReader.Core.Client.OtherAppRunning -= ClientOnOtherAppRunning;
      WindowModel.Instance.Dispose();
      Core.Client.Close();
    }
  }
}