using System.Threading.Tasks;
using System.Windows;
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
      window.ContentRendered += (sender, args) => Task.Run(() => Core.Client.Start(this));
      window.ShowDialog();
    }

    public Initialize(Window view) : base(view)
    {
      
    }
  }
}