using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using MangaReader.ViewModel.Commands;

namespace MangaReader
{
  /// <summary>
  /// Логика взаимодействия для App.xaml
  /// </summary>
  public partial class App : Application
  {
    private void App_OnExit(object sender, ExitEventArgs e)
    {
      new ExitCommand().Execute(sender);
    }

    private void App_OnStartup(object sender, StartupEventArgs e)
    {
      if (Environment.GetCommandLineArgs().Contains("-t"))
        ShowConsoleWindow();

      AppDomain.CurrentDomain.AssemblyResolve += LibSubfolderResolve;

      Client.Run();
    }

    private Assembly LibSubfolderResolve(object sender, ResolveEventArgs args)
    {
      var libName = args.Name;
      if (libName.Contains(','))
        libName = libName.Substring(0, libName.IndexOf(','));
      libName = libName + ".dll";
      var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib");
      var file = new DirectoryInfo(path).GetFiles().SingleOrDefault(f => f.Name == libName);
      if (file == null)
        return null;
      return Assembly.LoadFile(file.FullName);
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
