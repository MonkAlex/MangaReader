using MangaReader.Services;
using MangaReader.ViewModel;

namespace MangaReader
{
  public static class Client
  {
    public static void Run()
    {
      var model = new Initialize(new Converting());
      model.Show();

      WindowModel.Instance.Show();
      //var mainwindow = true ? new Table() as Window : new UI.MainForm.Blazard();
    }

    public static void Close()
    {
      WindowModel.Instance.Dispose();
      Core.Client.Close();
    }
  }
}