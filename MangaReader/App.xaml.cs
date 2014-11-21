using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using MangaReader.Services;

namespace MangaReader
{
  /// <summary>
  /// Логика взаимодействия для App.xaml
  /// </summary>
  public partial class App : Application
  {
    private void App_OnExit(object sender, ExitEventArgs e)
    {
      Settings.Save();
      Library.Save();
      Environment.Exit(0);
    }

    private void App_OnStartup(object sender, StartupEventArgs e)
    {
      AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
      if (Environment.GetCommandLineArgs().Contains("-t"))
        ShowConsoleWindow();
    }

    static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
    {
      var thisAssembly = Assembly.GetExecutingAssembly();
      var resourceNames = thisAssembly.GetManifestResourceNames();
      var subname = string.Format(".{0}.dll", args.Name.Substring(0, args.Name.IndexOf(',')));
      var resourceName = resourceNames.FirstOrDefault(s => s.EndsWith(subname)) ??
                         resourceNames.First(s => s.EndsWith(subname.Remove(0, 1)));
      using (var stream = thisAssembly.GetManifestResourceStream(resourceName))
      {
        var block = new byte[stream.Length];
        stream.Read(block, 0, block.Length);
        return Assembly.Load(block);
      }
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
