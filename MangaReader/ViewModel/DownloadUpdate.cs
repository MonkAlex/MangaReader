using System.Threading.Tasks;
using System.Windows;
using MangaReader.Update;

namespace MangaReader.ViewModel
{
  public class DownloadUpdate : ProcessModel
  {
    public DownloadUpdate(Window view) : base(view)
    {

    }

    public void Show()
    {
      view.ContentRendered += (sender, args) => Task.Run(() => Updater.StartUpdate(this));
      view.ShowDialog();
    }
  }
}