using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MangaReader.ViewModel
{
  public class BaseViewModel : INotifyPropertyChanged
  {
    public virtual void Load()
    {
      
    }

    public virtual void Unload()
    {
      
    }

    public BaseViewModel(FrameworkElement view)
    {
      view.Loaded += (o, a) => this.Load();
      view.Unloaded += (o, a) => this.Unload();

      // Таки это наверно лучше в событие типа Show.
      view.DataContext = this;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}