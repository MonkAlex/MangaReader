using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;
using System.Windows.Threading;
using MangaReader.Core.ApplicationControl;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Services;
using MangaReader.UI.Services;
using MangaReader.ViewModel;
using MangaReader.ViewModel.Commands;
using WindowState = System.Windows.WindowState;

namespace MangaReader
{
  public static class Client
  {
    const string AddManga = nameof(MangaReader.Core.ApplicationControl.Messages.AddManga);

    public static Dispatcher Dispatcher { get { return Application.Current.Dispatcher; } }

    public static void Run()
    {
      ViewResolver.Instance.ViewInit();
      MangaReader.Core.Client.OtherAppRunning += ClientOnOtherAppRunning;
      MangaReader.Core.Client.ClientBeenClosed += ClientOnClientBeenClosed;

      // Извлечение текущего списка часто используемых элементов
      var jumpList = new JumpList();
      JumpList.SetJumpList(App.Current, jumpList);

      // Добавление нового объекта JumpPath для файла в папке приложения
      var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
      if (File.Exists(path))
      {
        JumpTask jumpTask = new JumpTask
        {
          CustomCategory = "Манга",
          Title = "Добавить мангу",
          ApplicationPath = path,
          IconResourcePath = path,
          Arguments = AddManga
        };
        jumpList.JumpItems.Add(jumpTask);
      }

      // Обновление списка часто используемых элементов
      jumpList.Apply();
      
      var model = new Initialize();
      model.Show();

      WindowModel.Instance.Show();
    }

    internal static void ClientOnClientBeenClosed(object sender, EventArgs eventArgs)
    {
      if (Environment.GetCommandLineArgs().Any(a => a == AddManga))
        Core.ApplicationControl.Client.Run(Core.NHibernate.Repository.GetDatabaseUniqueId().ToString("D"), Messages.AddManga);
    }

    private static void ClientOnOtherAppRunning(object sender, string s)
    {
      if (!Messages.TryParse(s, true, out Messages message))
        return;

      switch (message)
      {
        case Messages.Activate:
          Dispatcher.Invoke(() =>
          {
            var window = WindowHelper.Owner;

            var state = WindowState.Normal;
            if (window.WindowState != WindowState.Minimized)
            {
              state = window.WindowState;
              window.WindowState = WindowState.Minimized;
            }

            if (window.WindowState == WindowState.Minimized)
              window.WindowState = state;

            if (window.Owner != null)
              window.Owner.Activate();

            window.Activate();
          });
          break;
        case Messages.AddManga:
          Dispatcher.Invoke(() =>
          {
            var library = WindowHelper.Library;
            if (library != null)
              new AddNewMangaCommand(library).Execute(null);
          });
          break;
        case Messages.Close:
          Dispatcher.Invoke(() =>
          {
            new ExitCommand().Execute(null);
          });
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public static void Close()
    {
      MangaReader.Core.Client.OtherAppRunning -= ClientOnOtherAppRunning;
      MangaReader.Core.Client.ClientBeenClosed -= ClientOnClientBeenClosed;
      WindowModel.Instance.Dispose();
      Core.Client.Close();
    }
  }
}