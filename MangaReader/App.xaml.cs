using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace MangaReader
{
  /// <summary>
  /// Логика взаимодействия для App.xaml
  /// </summary>
  public partial class App : Application
  {
    private void App_OnExit(object sender, ExitEventArgs e)
    {
      Client.Close();
      Current.Shutdown(0);
    }

    private void App_OnStartup(object sender, StartupEventArgs e)
    {
      if (Environment.GetCommandLineArgs().Contains("-t"))
        ShowConsoleWindow();

      AppDomain.CurrentDomain.AssemblyResolve += Core.ResolveAssembly.ResolveInternalAssembly;
      Client.Run();
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
