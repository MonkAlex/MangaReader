using System.Windows;
using MangaReader.Services;
using MangaReader.Services.Config;
using MangaReader.ViewModel;

namespace MangaReader
{
  public static class Client
  {
    public static void Run()
    {
      var model = new Initialize(new Converting());
      model.Show();

      var mainwindow = true ? new Table() as Window : new UI.MainForm.Blazard();
      mainwindow.ShowDialog();
    }

    public static int Close()
    {
      ConfigStorage.Instance.Save();
      return 0;
    }
  }
}