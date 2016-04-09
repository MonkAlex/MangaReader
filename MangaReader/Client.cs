using MangaReader.UI.Services;
using MangaReader.ViewModel;

namespace MangaReader
{
  public static class Client
  {
    public static void Run()
    {
      ViewResolver.Instance.ViewInit();

      var model = new Initialize();
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