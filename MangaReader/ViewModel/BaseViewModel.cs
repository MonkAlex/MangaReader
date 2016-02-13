using System.Windows;

namespace MangaReader.ViewModel
{
  public class BaseViewModel : Primitive.NotifyPropertyChanged
  {
    protected internal FrameworkElement view;

    public virtual void Load()
    {

    }

    public virtual void Unload()
    {

    }

    public virtual void Show()
    {
      view.DataContext = this;
    }

    public BaseViewModel(FrameworkElement view)
    {
      view.Loaded += (o, a) => this.Load();
      view.Unloaded += (o, a) => this.Unload();
      this.view = view;
    }
  }
}