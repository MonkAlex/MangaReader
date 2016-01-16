using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using MangaReader.Core;
using MangaReader.Services;
using MangaReader.Services.Config;
using MangaReader.Update;
using MangaReader.ViewModel;

namespace MangaReader
{
  /// <summary>
  /// Логика взаимодействия для App.xaml
  /// </summary>
  public partial class App : Application
  {
    private void App_OnExit(object sender, ExitEventArgs e)
    {
      ConfigStorage.Instance.Save();
      Environment.Exit(0);
    }

    private void App_OnStartup(object sender, StartupEventArgs e)
    {
      if (Environment.GetCommandLineArgs().Contains("-t"))
        ShowConsoleWindow();

      var model = new Initialize(new Converting());
      model.Show();

      var mainwindow = true ? new Table() as Window : new UI.MainForm.Blazard();
      mainwindow.ShowDialog();
    }

    #region Консоль

    public static void ShowConsoleWindow()
    {
      var handle = GetConsoleWindow();

      if (handle == IntPtr.Zero)
      {
        AllocConsole();
      }
      else
      {
        ShowWindow(handle, SW_SHOW);
      }

      Console.CancelKeyPress += (sender, args) =>
      {
        args.Cancel = true;
        HideConsoleWindow();
      };
    }

    public static void HideConsoleWindow()
    {
      var handle = GetConsoleWindow();

      ShowWindow(handle, SW_HIDE);
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;

    #endregion

  }
}
