using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using MangaReader.Account;
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
      Environment.Exit(0);
    }

    private void App_OnStartup(object sender, StartupEventArgs e)
    {
      this.DispatcherUnhandledException += (o, a) => Log.Exception(a.Exception);
      AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
      if (Environment.GetCommandLineArgs().Contains("-t"))
        ShowConsoleWindow();

      Update.Initialize();

      var isSingle = false;
      var mtx = new System.Threading.Mutex(true, "5197317b-a6f6-4a6c-a336-6fbf8642b7bc", out isSingle);
      if (!isSingle)
      {
        Log.Add("Программа уже запущена.");
        Environment.Exit(1);
      }

      Mapping.Environment.Initialize();
      Converter.Convert(true);
      Grouple.LoginWhile();

      var mainwindow = true ? new Table() as Window : new UI.MainForm.Blazard();
      mainwindow.ShowDialog();
    }

    static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
    {
      var thisAssembly = Assembly.GetExecutingAssembly();
      var resourceNames = thisAssembly.GetManifestResourceNames();
      var assemblyFile = string.Format(".{0}.dll", args.Name.Substring(0, args.Name.IndexOf(',')));
      var resourceName = resourceNames.FirstOrDefault(s => s.EndsWith(assemblyFile));
      if (string.IsNullOrWhiteSpace(resourceName))
      {
        assemblyFile = assemblyFile.Remove(0, 1);
        resourceName = resourceNames.First(s => s.EndsWith(assemblyFile));
      }

      using (var stream = thisAssembly.GetManifestResourceStream(resourceName))
      {
        var block = new byte[stream.Length];
        stream.Read(block, 0, block.Length);
        try
        {
          return Assembly.Load(block);
        }
        catch (FileLoadException)
        {
          assemblyFile = assemblyFile.TrimStart('.');
          File.WriteAllBytes(assemblyFile, block);
          return Assembly.LoadFrom(assemblyFile);
        }
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
