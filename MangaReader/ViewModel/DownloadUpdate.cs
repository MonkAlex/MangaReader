using System.Threading.Tasks;
using System.Windows;
using MangaReader.Update;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class DownloadUpdate : ProcessModel
  {
    public DownloadUpdate(Window view) : base(view)
    {

    }

    public override void Show()
    {
      base.Show();
      window.ContentRendered += (sender, args) => Task.Run(() => Updater.StartUpdate(this));
      window.ShowDialog();
    }
  }
}