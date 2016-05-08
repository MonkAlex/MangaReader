using System.Threading.Tasks;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class Initialize : ProcessModel
  {
    public override void Load()
    {
      base.Load();
      Core.Client.Init();
    }

    public override void Show()
    {
      base.Show();
      var window = ViewService.Instance.TryGet<System.Windows.Window>(this);
      if (window != null)
      {
        window.ContentRendered += (sender, args) => Task.Run(() => Core.Client.Start(this));
        window.ShowDialog();
      }
    }
  }
}