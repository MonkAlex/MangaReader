using System.Threading.Tasks;
using System.Windows;

namespace MangaReader.ViewModel
{
  public class Initialize : ProcessModel
  {
    public override void Load()
    {
      base.Load();
      Core.Client.Init();
    }

    public void Show()
    {
      view.ContentRendered += (sender, args) => Task.Run(() => Core.Client.Start(this));
      view.ShowDialog();
    }

    public Initialize(Window view) : base(view)
    {
      
    }
  }
}