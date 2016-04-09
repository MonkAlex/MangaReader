using System.Threading.Tasks;
using MangaReader.Core.Update;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class DownloadUpdate : ProcessModel
  {
    public override void Show()
    {
      base.Show();

      var window = ViewService.Instance.TryGet(this);
      if (window != null)
      {
        window.ContentRendered += (sender, args) => Task.Run(() => Updater.StartUpdate(this));
        window.ShowDialog();
      }
    }
  }
}