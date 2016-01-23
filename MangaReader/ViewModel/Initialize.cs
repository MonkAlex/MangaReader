using System.Threading.Tasks;
using System.Windows;
using MangaReader.Core;

namespace MangaReader.ViewModel
{
  public class Initialize : ProcessModel
  {
    public override void Load()
    {
      base.Load();
      Client.Init();
    }

    public void Show()
    {
      view.ContentRendered += (sender, args) => Task.Run(() => Client.Start(this));
      view.ShowDialog();
    }

    public Initialize(Window view) : base(view)
    {
      
    }
  }
}